﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Slaysher.Game.Entities;
using Slaysher.Game.Scenes;
using Slaysher.Game.World.Objects;

using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Game.World;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

namespace Slaysher.Network
{
    public class Client
    {
        private readonly ConcurrentQueue<Packet> _packetsToSend = new ConcurrentQueue<Packet>();

        public GameScene GameScene { get; set; }
        
        private bool _running;
        private readonly string _userName;

        private readonly Socket _socket;

        private Task _sendTask;

        private ByteQueue _receiveBufferQueue;
        private ByteQueue _readingBufferQueue;
        private readonly ByteQueue _fragPackets;

        private readonly byte[] _recvBuffer = new byte[2048];

        private readonly SocketAsyncEventArgs _socketAsyncArgs;

        private readonly object _queueLock = new object();
        private readonly Thread _receiveQueueReader;

        private readonly AutoResetEvent _recv = new AutoResetEvent(true);

        public bool WaitInitialPositionRequest = true;
        public object WaitInitialPositionRequestLook = new object();

        private int _sends;
        private int _sendRunning;

        public int DebugReceivedPattern = 0;

        public Client(GameScene gameScene)
        {
            GameScene = gameScene;
            PacketMap.Initialize();

            _userName = gameScene.Engine.Username;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _receiveBufferQueue = new ByteQueue();
            _readingBufferQueue = new ByteQueue();
            _fragPackets = new ByteQueue();
            _socketAsyncArgs = new SocketAsyncEventArgs();
            _receiveQueueReader = new Thread(ProcessReadQueue);
        }

        public void Start(IPEndPoint ipEndpoint)
        {
            _running = true;

            try
            {
                _socket.Connect(ipEndpoint);
            }
            catch (Exception)
            {
                Console.WriteLine("Error while connecting to Slaysher Server");
            }

            _socketAsyncArgs.Completed += RecvCompleted;
            _socketAsyncArgs.SetBuffer(_recvBuffer, 0, 2048);
            _receiveQueueReader.Start();
            Task.Factory.StartNew(RecvPacket);

            HandshakePacket handshake = new HandshakePacket {Username = _userName};
            SendPacket(handshake);
        }

        public void SendPacket(Packet packet)
        {
            _packetsToSend.Enqueue(packet);

            int sendRunning = Interlocked.CompareExchange(ref _sendRunning, 1, 0);
            if (sendRunning == 0)
            {
                _sendTask = Task.Factory.StartNew(Send);
            }
        }

        private void RecvPacket()
        {
            if (!_running)
            {
                Dispose();
                return;
            }

            bool pending = _socket.ReceiveAsync(_socketAsyncArgs);
            if (!pending)
            {
                RecvCompleted(null, _socketAsyncArgs);
            }
        }

        private void Send()
        {
            int sends = Interlocked.Increment(ref _sends);

            if (sends > 1)
            {
                Console.WriteLine("Multiple Packets to send");
            }
            if (!_running)
            {
                return;
            }

            int count = _packetsToSend.Count;
            for (int i = 0; i < count; ++i)
            {
                if (!_running)
                {
                    return;
                }

                Packet packet;
                _packetsToSend.TryDequeue(out packet);

                if (packet == null)
                {
                    return;
                }

                packet.Write();

                byte[] data = packet.GetBuffer();

                try
                {
                    _socket.Send(data);
                }
                catch (Exception)
                {
                    if (_running)
                    {
                        Dispose();
                    }
                }
            }

            Interlocked.Exchange(ref _sendRunning, 0);
            Interlocked.Decrement(ref _sends);
            if (_running && !_packetsToSend.IsEmpty)
            {
                int running = Interlocked.Exchange(ref _sendRunning, 1);
                if (running == 0)
                {
                    _sendTask = Task.Factory.StartNew(Send);
                }
            }
        }

        public void Dispose()
        {
            _running = false;
            _recv.Set();
            _receiveQueueReader.Abort();

            //if (_globalTimer != null)
            //{
            //    _globalTimer.Change(Timeout.Infinite, Timeout.Infinite);
            //    _globalTimer = null;
            //}

            if (_socket.Connected)
                _socket.Shutdown(SocketShutdown.Both);

            _socket.Close();
        }

        private void RecvCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                if (_running)
                {
                    Dispose();
                }
                return;
            }

            if (e.BytesTransferred > 0)
            {
                lock (_queueLock)
                    _receiveBufferQueue.Enqueue(e.Buffer, 0, e.BytesTransferred);
                _recv.Set();
                RecvPacket();

            }
        }

        private void ProcessReadQueue()
        {
            while (_recv.WaitOne())
            {
                ByteQueue temp;
                lock (_queueLock)
                {
                    temp = _receiveBufferQueue;
                    _receiveBufferQueue = _readingBufferQueue;
                }

                _readingBufferQueue = temp;

                int length = _fragPackets.Size + _readingBufferQueue.Size;

                while (length > 0)
                {
                    byte packetType = _fragPackets.Size > 0
                                          ? _fragPackets.GetPacketId()
                                          : _readingBufferQueue.GetPacketId();

                    ClientPacketHandler handler = PacketHandlers.GetHandler((PacketType) packetType);

                    if (handler == null)
                    {
                        byte[] unhandled = GetBufferToBeRead(length);
                        Console.WriteLine("Received unknown packet! Id:{0}", packetType);
                        Console.WriteLine("Fehler {0}", BitConverter.ToString(unhandled));
                        length = 0;
                    }
                    else if (handler.Length == 0)
                    {
                        byte[] data = GetBufferToBeRead(length);
                        Console.WriteLine("Klappt {0}", BitConverter.ToString(data));

                        if (length >= handler.MinimumLength)
                        {
                            PacketReader reader = new PacketReader(data, length);

                            handler.OnReceive(this, reader);
                            if (reader.Failed)
                            {
                                EnqueueFragment(data);
                                length = 0;
                            }
                            else
                            {
                                _readingBufferQueue.Enqueue(data, reader.Index, data.Length - reader.Index);
                                length = _readingBufferQueue.Length;
                            }
                        }
                        else
                        {
                            EnqueueFragment(data);
                            length = 0;
                        }
                    }
                    else if (length >= handler.Length)
                    {
                        byte[] data = GetBufferToBeRead(handler.Length);
                        /*using (StreamWriter sw = new StreamWriter(String.Format("recv_packets{0}.log", _userName), true))
                        {
                            sw.WriteLine("Fixed length: {0}", BitConverter.ToString(data));
                        }*/
                        PacketReader reader = new PacketReader(data, handler.Length);

                        handler.OnReceive(this, reader);

                        // If we failed it's because the packet is wrong
                        if (reader.Failed)
                        {
                            Dispose();
                            length = 0;
                        }
                        else
                        {
                            if (_fragPackets.Length > 0)
                                throw new Exception("Fragpackets must be empy here!");
                            length = _readingBufferQueue.Length;
                        }
                    }
                    else
                    {
                        /*using (StreamWriter sw = new StreamWriter(String.Format("recv_packets{0}.log", _userName), true))
                        { */
                        byte[] data = GetBufferToBeRead(length);
                        //sw.WriteLine("Fragmented fixed: {0}", BitConverter.ToString(data));
                        EnqueueFragment(data);
                        length = 0;
                        //}
                    }
                }
            }
        }

        private void EnqueueFragment(byte[] data)
        {
            int fragPacketWaiting = _fragPackets.Length;
            // We are waiting for more data than possible
            if (fragPacketWaiting > 81920)
                Dispose();
            else
                _fragPackets.Enqueue(data, 0, data.Length);
        }

        public void WaitForInitialPositionRequest() {
            lock (WaitInitialPositionRequestLook)
            {
                if (WaitInitialPositionRequest)
                {
                    Monitor.Wait(WaitInitialPositionRequestLook);
                }
            }
        }

        private byte[] GetBufferToBeRead(int length)
        {
            int availableData = _fragPackets.Size + _readingBufferQueue.Size;

            if (length > availableData)
                return null;

            byte[] data = new byte[length];

            int fromFrag = length >= _fragPackets.Size ? _fragPackets.Size : length;

            _fragPackets.Dequeue(data, 0, fromFrag);

            int fromProcessed = length - fromFrag;

            _readingBufferQueue.Dequeue(data, fromFrag, fromProcessed);

            return data;
        }

        public static void HandleHandshake(Client client, HandshakePacket hp)
        {
            //Handle Handshake
            Console.WriteLine("Received Handshake Packet back from Server :))");
        }

        public static void HandleKeepAlive(Client client, KeepAlivePacket ap)
        {
            //Respond KeepAlive Packet to Server
            Console.WriteLine("Received Keep Alive");
            client.SendPacket(new KeepAlivePacket {TimeStamp = ap.TimeStamp});

            lock (client.WaitInitialPositionRequestLook)
            {
                if (client.WaitInitialPositionRequest)
                {
                    client.WaitInitialPositionRequest = false;
                    Monitor.PulseAll(client.WaitInitialPositionRequestLook);
                }
            }
        }

        public static void HandlePatternPacket(Client client, PatternPacket pp)
        {
            Console.WriteLine("Received Pattern Packet: " + client.DebugReceivedPattern++);

            //Retrieve Pattern Texture
            Pattern newPattern = new Pattern(new Vector3(pp.X, 0, pp.Y),
                                             client.GameScene.LoadPatternTexture(pp.TextureId));
            client.GameScene.Pattern.Add(pp.PatternId, newPattern);
        }

        public void HandleEntitySpawn(EntitySpawnPacket esp)
        {
            //FIXME: Entities must have more details
            IEntity entity;
            if (!GameScene.Entities.TryGetValue(esp.EntityId, out entity))
            {
                entity = new ClientPlayer(this)
                {
                    Id = esp.EntityId,
                    Health = esp.Health,
                    Nickname = esp.Nickname,
                    Position = new WorldPosition(esp.X, esp.Y)
                };
                GameScene.Entities.Add(entity.Id, entity);
            }
            else
            {
                // FIXME: update the entetie here
            }
        }

        public void HandleEntityDespawn(EntityDespawnPacket edp)
        {
            GameScene.Entities.Remove(edp.EntityId);
        }

        public void Move(int entityId, WorldPosition position, float direction, float speed)
        {
            IEntity entity;
            if (!GameScene.Entities.TryGetValue(entityId, out entity))
            {
                Console.WriteLine("Couldn't find entity with id" + entityId);
                return;
            }

            entity.StopMoving(null);
            entity.Position = position;
            if (speed > 0)
            {
                entity.PrepareToMove(direction, speed);
            }
        }

        public void HandlePlayerInfo(PlayerInfoPacket pip)
        {
            Console.WriteLine("Received Player Info Packet");
            ClientPlayer player = new ClientPlayer(this) {
                Id = pip.PlayerId,
                Nickname = pip.Nickname,
                Health = pip.Health,
                Position = new WorldPosition(pip.X, pip.Y),
                SpeedMeterPerMillisecond = pip.Speed
            };
            // FIXME: ModelScaling should be dynamic, model depending and influencable by the server
            player.ModelScaling = 1f / 512f;

            if (GameScene.Entities.ContainsKey(player.Id))
            {
                Console.WriteLine("Tried to add new Player to Entity List, but already exists!");
            }
            else { 
                GameScene.Entities.Add(player.Id, player);
            }
            // first PlayerInfoPacket is the Player
            if (GameScene.Player == null)
            {
                GameScene.Player = player;
            }
        }

        public void HandlePlayerPosition(PlayerPositionPacket ppp)
        {
            Console.WriteLine("Received Player Position Packet");
            if (GameScene.Player != null)
            {
                WorldPosition wp = new WorldPosition(ppp.X, ppp.Y);
                GameScene.Player.Position = wp;
            }
            else
            {
                Console.WriteLine("Error. Player should exist");
            }
        }
    }
}