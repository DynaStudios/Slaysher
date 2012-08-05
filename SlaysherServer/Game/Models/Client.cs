﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

using SlaysherServer.Network;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public volatile bool Running = true;

        //Network Buffers
        private ByteQueue _currentBuffer;

        private ByteQueue _processedBuffer;

        internal ByteQueue FragPackets { get; set; }

        public static SocketAsyncEventArgsPool SendSocketEventPool = new SocketAsyncEventArgsPool(10);
        public static SocketAsyncEventArgsPool RecvSocketEventPool = new SocketAsyncEventArgsPool(10);
        public static BufferPool RecvBufferPool = new BufferPool("Receive", 2048, 2048);

        private byte[] _recvBuffer;
        private SocketAsyncEventArgs _sendSocketEvent;
        private SocketAsyncEventArgs _recvSocketEvent;

        private bool _sendSystemDisposed;
        private bool _recvSystemDisposed;

        private readonly object _disposeLock = new object();

        //Client specific Stuff
        public int ClientId { get; private set; }

        private DateTime _nextActivityCheck;

        public long LastSendKeepAliveStamp { get; set; }

        public bool IsLoggingIn { get; set; }

        public Server Server { get; set; }

        public Player Player { get; private set; }

        private readonly Socket _socket;

        public Client(int nextClientId, Server server, Socket socket)
        {
            IsLoggingIn = false;

            ClientId = nextClientId;
            Server = server;
            _socket = socket;

            _currentBuffer = new ByteQueue();
            _processedBuffer = new ByteQueue();
            FragPackets = new ByteQueue();

            _nextActivityCheck = DateTime.Now + TimeSpan.FromSeconds(30);
        }

        public void Start()
        {
            _sendSocketEvent = SendSocketEventPool.Pop();
            _recvSocketEvent = RecvSocketEventPool.Pop();
            _recvBuffer = RecvBufferPool.AcquireBuffer();

            _recvSocketEvent.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
            _recvSocketEvent.Completed += RecvCompleted;
            _sendSocketEvent.Completed += SendCompleted;

            Task.Factory.StartNew(RecvStart);
        }

        internal void Stop()
        {
            MarkToDispose();
            DisposeRecvSystem();
            DisposeSendSystem();
        }

        public void MarkToDispose()
        {
            lock (_disposeLock)
            {
                if (Running)
                {
                    Running = false;
                }
            }
        }

        public void Dispose()
        {
            if (Player != null)
            {
                Save();

                Server.RemoveClient(this);

                Client[] nearbyClients = Server.GetNearbyPlayers(Player.Position).ToArray();

                foreach (var client in nearbyClients)
                {
                    if (client != this)
                    {
                        EntityDespawnPacket dp = new EntityDespawnPacket {EntityId = client.ClientId};
                        dp.Write();
                        byte[] data = dp.GetBuffer();
                        client.SendSync(data);
                    }
                }

                Running = false;
            }
            else
            {
                Running = false;
                Server.RemoveClient(this);
                Server.FreeConnectionSlot();
            }

            RecvBufferPool.ReleaseBuffer(_recvBuffer);
            SendSocketEventPool.Push(_sendSocketEvent);
            RecvSocketEventPool.Push(_recvSocketEvent);

            if (_socket.Connected)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException)
                {
                    // Ignore errors in socket shutdown (e.g. if client crashes there is a no connection error when trying to shutdown)
                }
            }
            _socket.Close();

            //GC.Collect();
        }

        internal void DisposeSendSystem()
        {
            lock (_disposeLock)
            {
                if (!_sendSystemDisposed)
                {
                    _sendSystemDisposed = true;
                    if (_recvSystemDisposed)
                    {
                        Server.ClientsToDispose.Enqueue(this);
                        Server.NetworkSignal.Set();
                    }
                }
            }
        }

        internal void DisposeRecvSystem()
        {
            lock (_disposeLock)
            {
                if (!_recvSystemDisposed)
                {
                    _recvSystemDisposed = true;
                    if (_sendSystemDisposed)
                    {
                        Server.ClientsToDispose.Enqueue(this);
                        Server.NetworkSignal.Set();
                    }
                }
            }
        }

        internal void Update(TimeSpan totalTime)
        {
            if (Player.ExecutePreparedMove(totalTime))
            {
                Console.WriteLine("Sending move");
                MovePacket mp = new MovePacket
                {
                    EntetyId = Player.Id,
                    Direction = Player.Direction,
                    Position = Player.Position,
                    Speed = Player.Speed,
                };

                SendPacket(mp);
            }
        }
    }
}