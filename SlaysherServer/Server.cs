﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using SlaysherNetworking.Game.World;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

using SlaysherServer.Database;
using SlaysherServer.Game;
using SlaysherServer.Game.Models;
using SlaysherServer.Network.Events;
using SlaysherServer.Network.Handler;

namespace SlaysherServer
{
    public class Server : IDisposable
    {
        public const int TICK_TIME = 50;
        public static int SightRadius = 10;
        private DateTime _startTime;

        private readonly Socket _listener;
        private readonly SocketAsyncEventArgs _acceptEventArgs;

        private int _nextClientId = 1;

        public static ConcurrentQueue<Client> RecvClientQueue = new ConcurrentQueue<Client>();
        public static ConcurrentQueue<Client> SendClientQueue = new ConcurrentQueue<Client>();

        public static ConcurrentQueue<Client> ClientsToDispose = new ConcurrentQueue<Client>();

        private Task _readClientsPackets;
        private Task _sendClientPackets;
        private Task _disposeClients;

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
            _startTime = DateTime.Now;
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

            _globalTick = new Timer(GlobalTickProc, null, TICK_TIME, TICK_TIME);

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
            TimeSpan runTime = DateTime.Now - _startTime;
            _serverTick++;
            World.Tick(_serverTick);

            Client[] clients = GetClients();
            Parallel.ForEach<Client>(clients, (client) =>
            {
                client.Update(runTime);
            });
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

                if (!ClientsToDispose.IsEmpty && (_disposeClients == null || _disposeClients.IsCompleted))
                {
                    _disposeClients = Task.Factory.StartNew(DisposeClients);
                }

                if (!SendClientQueue.IsEmpty && (_sendClientPackets == null || _sendClientPackets.IsCompleted))
                {
                    _sendClientPackets = Task.Factory.StartNew(ProcessSendQueue);
                }
            }
        }

        private void DisposeClients()
        {
            int count = ClientsToDispose.Count;
            while (!ClientsToDispose.IsEmpty)
            {
                Client client;
                if (!ClientsToDispose.TryDequeue(out client))
                    continue;

                client.Dispose();
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
                        packetType = client.FragPackets.GetPacketId();
                    else
                        packetType = bufferToProcess.GetPacketId();

                    PacketHandler handler = PacketHandlers.GetHandler((PacketType)packetType);

                    if (handler == null)
                    {
                        byte[] unhandledPacketData = GetBufferToBeRead(bufferToProcess, client, length);
                        Console.WriteLine("Unhandled packet arrived, id: {0}", unhandledPacketData[0]);
                        Console.WriteLine("Data:\r\n {0}", BitConverter.ToString(unhandledPacketData, 1));

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
                            client.MarkToDispose();
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
                        client.DisposeSendSystem();
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
                int clientId = Interlocked.Increment(ref _nextClientId);
                // FIXME: client muss noch aus datenbank geladen werden
                Client c = new Client(clientId, clientId, this, e.AcceptSocket);

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

        private int _clientDictChanges;
        private Client[] _clientsCache;

        public Client[] GetClients()
        {
            int changes = Interlocked.Exchange(ref _clientDictChanges, 0);
            if (_clientsCache == null || changes > 0)
                _clientsCache = Clients.Values.ToArray();

            return _clientsCache;
        }

        private void AddClient(Client c)
        {
            Clients.TryAdd(c.ClientId, c);
            Interlocked.Increment(ref _clientDictChanges);
        }

        public void RemoveClient(Client c)
        {
            Clients.TryRemove(c.ClientId, out c);
            Interlocked.Increment(ref _clientDictChanges);
        }

        internal void FreeConnectionSlot()
        {
            Interlocked.Increment(ref MaxClientConnections);
            NetworkSignal.Set();
        }

        internal void SendPacketBroadcast(Packet packet)
        {
            Client[] clients = GetClients();
            packet.SetShared(clients.Length);

            Parallel.ForEach(clients, client => client.SendPacket(packet));
        }

        internal void SendPacketToClientList(Packet packet, Client[] targetList, Client excludedClient = null)
        {
            if (targetList.Length == 0)
                return;

            if (targetList.Length == 1)
            {
                if (targetList[0] == excludedClient)
                    return;
                targetList[0].SendPacket(packet);
            }
            else
            {
                packet.SetShared(targetList.Length);

                Parallel.ForEach(targetList, client =>
                    {
                        if (excludedClient != client)
                        {
                            client.SendPacket(packet);
                        }
                        else
                        {
                            packet.Release();
                        }
                    });
            }

        }

        internal void SendPacketToNearbyPlayers(WorldPosition pos, Packet packet, Client excludedClient = null)
        {
            Client[] nearbyClients = GetNearbyPlayers(pos).ToArray();

            if (nearbyClients.Length == 0)
                return;

            packet.SetShared(nearbyClients.Length);

            Parallel.ForEach(nearbyClients, client =>
                {
                    if (excludedClient != client)
                    {
                        client.SendPacket(packet);
                    }
                    else
                    {
                        packet.Release();
                    }
                });
        }

        internal IEnumerable<Client> GetNearbyPlayers(WorldPosition pos)
        {
            int radius = SightRadius;
            foreach (Client c in GetClients())
            {
                int playerPatternX = (int) Math.Floor(c.Player.Position.X) >> 5;
                int playerPatternY = (int) Math.Floor(c.Player.Position.Y) >> 5;
                if (Math.Abs(pos.X - playerPatternX) <= radius && Math.Abs(pos.Y - playerPatternY) <= radius)
                    yield return c;
            }
        }

        public void Dispose()
        {
            if (DAO != null)
            {
                DAO.Dispose();
            }
        }
    }
}