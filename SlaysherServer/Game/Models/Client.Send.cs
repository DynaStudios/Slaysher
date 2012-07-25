using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public ConcurrentQueue<Packet> PacketsToBeSent = new ConcurrentQueue<Packet>();
        private int _timesEnqueuedForSend;

        internal void SendPacket(Packet packet)
        {
            if (!Running)
                return;

            PacketsToBeSent.Enqueue(packet);

            int newValue = Interlocked.Increment(ref _timesEnqueuedForSend);

            if (newValue == 1)
            {
                Server.SendClientQueue.Enqueue(this);
            }

            Server.NetworkSignal.Set();

            //Logger.Log(Chraft.LogLevel.Info, "Sending packet: {0}", packet.GetPacketType().ToString());
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (!Running)
                DisposeSendSystem();
            else if (e.SocketError != SocketError.Success)
            {
                MarkToDispose();
                DisposeSendSystem();
                _nextActivityCheck = DateTime.MinValue;
            }
            else
            {
                if (DateTime.Now + TimeSpan.FromSeconds(5) > _nextActivityCheck)
                    _nextActivityCheck = DateTime.Now + TimeSpan.FromSeconds(5);
                SendStart();
            }
        }

        private void Send_Async(byte[] data)
        {
            if (!Running || !_socket.Connected)
            {
                DisposeSendSystem();
                return;
            }

            _sendSocketEvent.SetBuffer(data, 0, data.Length);
            bool pending = _socket.SendAsync(_sendSocketEvent);
            if (!pending)
                SendCompleted(null, _sendSocketEvent);
        }

        private void SendSync(byte[] data)
        {
            if (!Running || !_socket.Connected)
            {
                DisposeSendSystem();
                return;
            }
            try
            {
                _socket.Send(data, 0, data.Length, 0);
            }
            catch (Exception)
            {
                Stop();
            }
        }

        internal void SendStart()
        {
            if (!Running || !_socket.Connected)
            {
                DisposeSendSystem();
                return;
            }

            try
            {
                ByteQueue byteQueue = new ByteQueue();
                int length = 0;
                while (!PacketsToBeSent.IsEmpty && length <= 1024)
                {
                    Packet packet;
                    if (!PacketsToBeSent.TryDequeue(out packet))
                    {
                        Interlocked.Exchange(ref _timesEnqueuedForSend, 0);
                        return;
                    }

                    if (!packet.Shared)
                        packet.Write();

                    byte[] packetBuffer = packet.GetBuffer();
                    length += packetBuffer.Length;

                    byteQueue.Enqueue(packetBuffer, 0, packetBuffer.Length);
                    packet.Release();
                }

                if (byteQueue.Length > 0)
                {
                    byte[] data = new byte[length];
                    byteQueue.Dequeue(data, 0, data.Length);
                    Send_Async(data);
                }
                else
                {
                    Interlocked.Exchange(ref _timesEnqueuedForSend, 0);

                    if (!PacketsToBeSent.IsEmpty)
                    {
                        int newValue = Interlocked.Increment(ref _timesEnqueuedForSend);

                        if (newValue == 1)
                        {
                            Server.SendClientQueue.Enqueue(this);
                            Server.NetworkSignal.Set();
                        }
                    }
                }
            }
            catch (Exception)
            {
                MarkToDispose();
                DisposeSendSystem();
                //if (packet != null)
                //Logger.Log(LogLevel.Error, "Sending packet: {0}", packet.ToString());
                //Logger.Log(LogLevel.Error, e.ToString());

                // TODO: log something?
            }
        }

        private void SendAsync(byte[] data)
        {
            if (!Running || !_socket.Connected)
            {
                DisposeSendSystem();
                return;
            }

            _sendSocketEvent.SetBuffer(data, 0, data.Length);
            bool pending = _socket.SendAsync(_sendSocketEvent);
            if (!pending)
                SendCompleted(null, _sendSocketEvent);
        }
    }
}