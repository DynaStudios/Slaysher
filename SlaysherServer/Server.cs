using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using SlaysherNetworking.Network;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

using SlaysherServer.Database;
using SlaysherServer.Game;
using SlaysherServer.Game.Models;
using SlaysherServer.Network.Events;
using SlaysherServer.Network.Handler;

namespace SlaysherServer
{
    public class Server
    {
        private readonly Socket _listener;
        private readonly SocketAsyncEventArgs _acceptEventArgs;

        private int _nextClientId = 1;

        public static ConcurrentQueue<Client> RecvClientQueue = new ConcurrentQueue<Client>();
        public static ConcurrentQueue<Client> SendClientQueue = new ConcurrentQueue<Client>();

        public static ConcurrentQueue<Client> ClientsToDispose = new ConcurrentQueue<Client>();

        private Task _readClientsPackets;
        private Task _sendClientPackets;

        //Server Properties
        private bool _running = true;

        public int MaxClientConnections = 10;

        public ConcurrentDictionary<int, Client> Clients { get; private set; }

        public AutoResetEvent NetworkSignal = new AutoResetEvent(true);
        private int _asyncAccepts;

        private Timer _globalTick;
        private long _serverTick = 0;

        //Server own Eventhandler
        public event EventHandler<TcpEventArgs> BeforeAccept;

        public DAO DAO { get; private set; }

        public World World { get; set; }

        public Server()
        {
            //Network Setup
            PacketMap.Initialize();

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _acceptEventArgs = new SocketAsyncEventArgs();
            _acceptEventArgs.Completed += AcceptCompleted;

            DAO = new DAO();

            //Init World
            World = new World(this);

            //Vars Init
            Clients = new ConcurrentDictionary<int, Client>();
        }

        //Public Methods
        public void Run()
        {
            //Start Game World
            World.Start();

            //Start Network Layer
            for (int i = 0; i < 10; ++i)
            {
                Client.SendSocketEventPool.Push(new SocketAsyncEventArgs());
                Client.SendSocketEventPool.Push(new SocketAsyncEventArgs());
            }

            _globalTick = new Timer(GlobalTickProc, null, 50, 50);

            while (_running)
            {
                RunProc();
            }
        }

        /// <summary>
        /// Gets called 20 times every second
        /// </summary>
        /// <param name="state"></param>
        private void GlobalTickProc(object state)
        {
            _serverTick++;
            World.Tick(_serverTick);
        }

        //Eventhandler
        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            AcceptProcess(e);
        }

        //Private Methods
        private void RunProc()
        {
            IPAddress address = IPAddress.Parse("0.0.0.0");
            IPEndPoint endPoint = new IPEndPoint(address, 25104);

            _listener.Bind(endPoint);
            _listener.Listen(5);

            Console.WriteLine("Started Server and Listening...");

            RunNetwork();

            if (_running)
            {
                //Wait one second before restarting network
                Thread.Sleep(1000);
            }
        }

        private void RunNetwork()
        {
            while (NetworkSignal.WaitOne())
            {
                if (TryTakeConnectionSlot())
                    _listener.AcceptAsync(_acceptEventArgs);

                if (!RecvClientQueue.IsEmpty && (_readClientsPackets == null || _readClientsPackets.IsCompleted))
                {
                    _readClientsPackets = Task.Factory.StartNew(ProcessReadQueue);
                }

                /*

                if (!ClientsToDispose.IsEmpty && (_disposeClients == null || _disposeClients.IsCompleted))
                {
                    _disposeClients = Task.Factory.StartNew(DisposeClients);
                }
                 * */
                if (!SendClientQueue.IsEmpty && (_sendClientPackets == null || _sendClientPackets.IsCompleted))
                {
                    _sendClientPackets = Task.Factory.StartNew(ProcessSendQueue);
                }
            }
        }

        public bool TryTakeConnectionSlot()
        {
            int accepts = Interlocked.Exchange(ref _asyncAccepts, 1);
            if (accepts == 0)
            {
                int count = Interlocked.Decrement(ref MaxClientConnections);

                if (count >= 0)
                    return true;

                _asyncAccepts = 0;

                Interlocked.Increment(ref MaxClientConnections);
            }

            return false;
        }

        public static void ProcessReadQueue()
        {
            int count = RecvClientQueue.Count;

            Parallel.For(0, count, i =>
            {
                Client client;
                if (!RecvClientQueue.TryDequeue(out client))
                    return;

                if (!client.Running)
                    return;

                Interlocked.Exchange(ref client.TimesEnqueuedForRecv, 0);
                ByteQueue bufferToProcess = client.GetBufferToProcess();

                int length = client.FragPackets.Size + bufferToProcess.Size;
                while (length > 0)
                {
                    byte packetType = 0;

                    if (client.FragPackets.Size > 0)
                        packetType = client.FragPackets.GetPacketID();
                    else
                        packetType = bufferToProcess.GetPacketID();

                    //client.Logger.Log(Chraft.LogLevel.Info, "Reading packet {0}", ((PacketType)packetType).ToString());

                    Console.WriteLine("Try to resolve packet with id: " + packetType);
                    PacketHandler handler = PacketHandlers.GetHandler((PacketType)packetType);

                    if (handler == null)
                    {
                        byte[] unhandledPacketData = GetBufferToBeRead(bufferToProcess, client, length);

                        length = 0;
                    }
                    else if (handler.Length == 0)
                    {
                        byte[] data = GetBufferToBeRead(bufferToProcess, client, length);

                        if (length >= handler.MinimumLength)
                        {
                            PacketReader reader = new PacketReader(data, length);

                            handler.OnReceive(client, reader);

                            // If we failed it's because the packet isn't complete
                            if (reader.Failed)
                            {
                                EnqueueFragment(client, data);
                                length = 0;
                            }
                            else
                            {
                                bufferToProcess.Enqueue(data, reader.Index, data.Length - reader.Index);
                                length = bufferToProcess.Length;
                            }
                        }
                        else
                        {
                            EnqueueFragment(client, data);
                            length = 0;
                        }
                    }
                    else if (length >= handler.Length)
                    {
                        byte[] data = GetBufferToBeRead(bufferToProcess, client, handler.Length);

                        PacketReader reader = new PacketReader(data, handler.Length);

                        handler.OnReceive(client, reader);

                        // If we failed it's because the packet is wrong
                        if (reader.Failed)
                        {
                            //client.MarkToDispose();
                            length = 0;
                        }
                        else
                            length = bufferToProcess.Length;
                    }
                    else
                    {
                        byte[] data = GetBufferToBeRead(bufferToProcess, client, length);
                        EnqueueFragment(client, data);
                        length = 0;
                    }
                }
            });
        }

        private static void EnqueueFragment(Client client, byte[] data)
        {
            int fragPacketWaiting = client.FragPackets.Length;
            // We are waiting for more data than an uncompressed chunk, it's not possible
            if (fragPacketWaiting > 81920)
            {
                //client.Kick("Too much pending data to be read");
            }
            else
            {
                client.FragPackets.Enqueue(data, 0, data.Length);
            }
        }

        private static byte[] GetBufferToBeRead(ByteQueue processedBuffer, Client client, int length)
        {
            int availableData = client.FragPackets.Size + processedBuffer.Size;

            if (length > availableData)
                return null;

            int fromFrag;

            byte[] data = new byte[length];

            if (length >= client.FragPackets.Size)
                fromFrag = client.FragPackets.Size;
            else
                fromFrag = length;

            client.FragPackets.Dequeue(data, 0, fromFrag);

            int fromProcessed = length - fromFrag;

            processedBuffer.Dequeue(data, fromFrag, fromProcessed);

            return data;
        }

        public static void ProcessSendQueue()
        {
            int count = SendClientQueue.Count;

            Parallel.For(0, count, i =>
            {
                Client client;
                if (!SendClientQueue.TryDequeue(out client))
                    return;

                if (!client.Running)
                {
                    //client.DisposeSendSystem();
                    return;
                }

                client.SendStart();
            });
        }

        private void AcceptProcess(SocketAsyncEventArgs e)
        {
            if (OnBeforeAccept(e.AcceptSocket))
            {
                Console.WriteLine("Incoming Connection");
                Interlocked.Increment(ref _nextClientId);
                Client c = new Client(_nextClientId, this, e.AcceptSocket);

                c.Start();

                AddClient(c);
            }
            else
            {
                if (e.AcceptSocket.Connected)
                {
                    e.AcceptSocket.Shutdown(SocketShutdown.Both);
                }
                e.AcceptSocket.Close();
            }

            _acceptEventArgs.AcceptSocket = null;
            Interlocked.Exchange(ref _asyncAccepts, 0);
            NetworkSignal.Set();
        }

        private bool OnBeforeAccept(Socket socket)
        {
            if (BeforeAccept != null)
            {
                var e = new TcpEventArgs(socket);
                BeforeAccept.Invoke(this, e);
                return !e.Cancelled;
            }
            return true;
        }

        private void AddClient(Client c)
        {
            Clients.TryAdd(c.ClientId, c);
        }
    }
}