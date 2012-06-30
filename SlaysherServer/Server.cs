using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlaysherNetworking.Packets;
using SlaysherServer.Game.Models;
using SlaysherServer.Network;
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

        //Server own Eventhandler
        public event EventHandler<TcpEventArgs> BeforeAccept;

        public Server()
        {
            //Network Setup
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _acceptEventArgs = new SocketAsyncEventArgs();
            _acceptEventArgs.Completed += AcceptCompleted;

            //Vars Init
            Clients = new ConcurrentDictionary<int, Client>();
        }

        //Public Methods
        public void Run()
        {
            for (int i = 0; i < 10; ++i)
            {
                Client.SendSocketEventPool.Push(new SocketAsyncEventArgs());
                Client.SendSocketEventPool.Push(new SocketAsyncEventArgs());
            }

            while (_running)
            {
                RunProc();
            }
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
                //Do not touch :>
                /*
                string[] commands = bufferToProcess.GetCommands();

                if (commands.Length > 1)
                {
                    for (int u = 0; u < commands.Length - 1; u++)
                    {
                        int index = commands[u].IndexOf(' ');
                        var myEnum = (PacketType)Enum.Parse(typeof(PacketType), commands[u].Substring(0, index));

                        PacketHandler handler = PacketHandlers.GetHandler(myEnum);

                        if (handler != null)
                            handler.OnReceive(client, Encoding.UTF8.GetBytes(commands[u]));
                        else
                        {
                            //Unknown Command
                        }
                    }
                }
                 */
            });
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