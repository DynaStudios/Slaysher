﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Slaysher.Game.Scenes;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;
using SlaysherServer.Network;

namespace Slaysher.Network
{
    public class Client
    {
        private ConcurrentQueue<Packet> _packetsToSend = new ConcurrentQueue<Packet>();

        public GameSampleScene GameScene { get; set; }

        //private PacketWriter _packetWriter;
        private PacketReader _packetReader;

        private bool _running;
        private string _userName;

        private Socket _socket;

        private Task _sendTask;

        private ByteQueue _receiveBufferQueue;
        private ByteQueue _readingBufferQueue;
        private ByteQueue _fragPackets;

        private byte[] _recvBuffer = new byte[2048];

        private SocketAsyncEventArgs _socketAsyncArgs;

        private object _queueLock = new object();
        private Thread _receiveQueueReader;
        private Timer _globalTimer;

        private AutoResetEvent _recv = new AutoResetEvent(true);

        private int _time;

        public bool WaitInitialPositionRequest = true;

        private int Sends;
        private int SendRunning;

        public Client(GameSampleScene gameScene)
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
            catch (Exception e)
            {
                Console.WriteLine("Error while connecting to Slaysher Server");
            }

            _socketAsyncArgs.Completed += RecvCompleted;
            _socketAsyncArgs.SetBuffer(_recvBuffer, 0, 2048);
            _receiveQueueReader.Start();
            Task.Factory.StartNew(RecvPacket);

            //TODO: Send Handshake Packet here
            HandshakePacket handshake = new HandshakePacket(_userName);
            SendPacket(handshake);
        }

        private void SendPacket(Packet packet)
        {
            _packetsToSend.Enqueue(packet);

            int sendRunning = Interlocked.CompareExchange(ref SendRunning, 1, 0);
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
            int sends = Interlocked.Increment(ref Sends);

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
                catch (Exception e)
                {
                    if (_running)
                    {
                        Dispose();
                    }
                }
            }

            Interlocked.Exchange(ref SendRunning, 0);
            Interlocked.Decrement(ref Sends);
            if (_running && !_packetsToSend.IsEmpty)
            {
                int running = Interlocked.Exchange(ref SendRunning, 1);
                if (running == 0)
                {
                    _sendTask = Task.Factory.StartNew(Send);
                }
            }
        }

        public void Dispose()
        {
            try
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
            catch (Exception e)
            {
                throw e;
            }
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
                {
                    _receiveBufferQueue.Enqueue(e.Buffer, 0, e.BytesTransferred);
                    _recv.Set();
                    RecvPacket();
                }
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
                    byte packetType;

                    if (_fragPackets.Size > 0)
                    {
                        packetType = _fragPackets.GetPacketID();
                    }
                    else
                    {
                        packetType = _readingBufferQueue.GetPacketID();
                    }

                    ClientPacketHandler handler = PacketHandlers.GetHandler((PacketType)packetType);

                    if (handler == null)
                    {
                        Console.WriteLine("Received unknown packet!");
                        length = 0;
                    }
                    else if (handler.Length == 0)
                    {
                        byte[] data = GetBufferToBeRead(length);

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

        private byte[] GetBufferToBeRead(int length)
        {
            int availableData = _fragPackets.Size + _readingBufferQueue.Size;

            if (length > availableData)
                return null;

            int fromFrag;

            byte[] data = new byte[length];

            if (length >= _fragPackets.Size)
                fromFrag = _fragPackets.Size;
            else
                fromFrag = length;

            _fragPackets.Dequeue(data, 0, fromFrag);

            int fromProcessed = length - fromFrag;

            _readingBufferQueue.Dequeue(data, fromFrag, fromProcessed);

            return data;
        }

        internal static void HandleHandshake(Client client, HandshakePacket hp)
        {
            //Handle Handshake
            Console.WriteLine("Received Handshake Packet back from Server :))");
        }
    }
}