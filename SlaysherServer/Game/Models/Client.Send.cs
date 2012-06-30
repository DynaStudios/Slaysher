using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using SlaysherNetworking.Packets;
using SlaysherServer.Network;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public ConcurrentQueue<Packet> PacketsToBeSent = new ConcurrentQueue<Packet>();
        private int _timesEnqueuedForSend;

        public void SendPacket(Packet packet)
        {
            if (!Running)
                return;

            PacketsToBeSent.Enqueue(packet);

            int newValue = Interlocked.Increment(ref _timesEnqueuedForSend);

            if (newValue == 1)
            {
                Server.SendClientQueue.Enqueue(this);
            }

            _server.NetworkSignal.Set();
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            /*
            if (e.Buffer[0] == (byte)PacketType.Disconnect)
                e.Completed -= Disconnected;
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
                Send_Start();
            }
             */
        }

        internal void SendStart()
        {
            if (!Running || !_socket.Connected)
            {
                //DisposeSendSystem();
                return;
            }

            Packet packet = null;
            /*try
            {
                var byteQueue = new ByteQueue();
                int length = 0;
                while (!PacketsToBeSent.IsEmpty && length <= 1024)
                {
                    if (!PacketsToBeSent.TryDequeue(out packet))
                    {
                        Interlocked.Exchange(ref _timesEnqueuedForSend, 0);
                        return;
                    }

                    packet.Write();

                    byte[] packetBuffer = packet.Stream.GetBuffer();
                    length += packetBuffer.Length;

                    byteQueue.Enqueue(packetBuffer, 0, packetBuffer.Length);
                }

                if (byteQueue.Length > 0)
                {
                    var data = new byte[length];
                    byteQueue.Dequeue(data, 0, data.Length);
                    SendAsync(data);
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
                            _server.NetworkSignal.Set();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //DisposeSendSystem();

                // TODO: log something?
            }
             */
        }

        private void SendAsync(byte[] data)
        {
            if (!Running || !_socket.Connected)
            {
                //DisposeSendSystem();
                return;
            }

            _sendSocketEvent.SetBuffer(data, 0, data.Length);
            bool pending = _socket.SendAsync(_sendSocketEvent);
            if (!pending)
                Send_Completed(null, _sendSocketEvent);
        }
    }
}