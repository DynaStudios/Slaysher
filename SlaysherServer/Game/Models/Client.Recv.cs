using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SlaysherNetworking.Network;
using SlaysherNetworking.Packets;
using SlaysherServer.Network;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public int TimesEnqueuedForRecv;
        private readonly object _queueSwapLock = new object();
        private static List<PatternPacket> _pattern = _generateTestPattern();

        private void RecvStart()
        {
            if (!Running)
            {
                // DisposeRecvSystem();
                return;
            }

            if (!_socket.Connected)
            {
                //  Stop();
                return;
            }

            try
            {
                bool pending = _socket.ReceiveAsync(_recvSocketEvent);

                if (!pending)
                    RecvCompleted(null, _recvSocketEvent);
            }
            catch
            {
                //TODO:
                //Yo Patrick you should really catch this shit!
            }
        }

        private void RecvCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (Running)
            {
                if (DateTime.Now + TimeSpan.FromSeconds(5) > _nextActivityCheck)
                    _nextActivityCheck = DateTime.Now + TimeSpan.FromSeconds(5);
                RecvProcess(e);
            }
        }

        private void RecvProcess(SocketAsyncEventArgs e)
        {
            lock (_queueSwapLock)
                _currentBuffer.Enqueue(e.Buffer, 0, e.BytesTransferred);

            int newValue = Interlocked.Increment(ref TimesEnqueuedForRecv);

            if ((newValue - 1) == 0)
                Server.RecvClientQueue.Enqueue(this);

            _server.NetworkSignal.Set();

            RecvStart();
        }

        public ByteQueue GetBufferToProcess()
        {
            lock (_queueSwapLock)
            {
                ByteQueue temp = _currentBuffer;
                _currentBuffer = _processedBuffer;
                _processedBuffer = temp;
            }

            return _processedBuffer;
        }

        private static List<PatternPacket> _generateTestPattern()
        {
            List<PatternPacket> list = new List<PatternPacket>();
            return list;
        }

        public static void HandleHandshake(Client client, HandshakePacket packet)
        {
            if (packet.Username != null && packet.Password != null && !client.IsLoggingIn)
            {
                client.IsLoggingIn = true;
                // check for baned users
                // if (banlist.Contains(client)) {
                //    client.SendPacket(new KickPacket() { message = "You'r BANNED!!!! Get lost!" };
                //    return;
                // }

                //TODO: Debugging. Remove this if tested
                Console.WriteLine("Received Login for User " + packet.Username + " with password " + packet.Password);

                Console.WriteLine("Send Handshake back!");
                //Remove the password from packet.
                packet.Password = "blablabla";
                client.SendPacket(packet);
                foreach (PatternPacket pattern in _pattern)
                {
                    client.SendPacket(pattern);
                }

                //Send some sample Pattern to draw
                client.SendPacket(new PatternPacket { PatternID = 1, TextureID = 1, X = 0, Y = 0 });
                client.SendPacket(new PatternPacket { PatternID = 2, TextureID = 2, X = 50, Y = 0 });
                client.SendPacket(new PatternPacket { PatternID = 3, TextureID = 3, X = 50, Y = 50 });
                client.SendPacket(new PatternPacket { PatternID = 4, TextureID = 4, X = 0, Y = 50 });

                client.LastSendKeepAliveStamp = DateTime.Now.Ticks;

                Console.WriteLine("Finished Init. Send KeepAlive");
                KeepAlivePacket keepAlive = new KeepAlivePacket { TimeStamp = client.LastSendKeepAliveStamp };
                client.SendPacket(keepAlive);
            }
        }

        public static void HandleKeepAlive(Client client, KeepAlivePacket ap)
        {
            if (ap.TimeStamp + 3000 > DateTime.Now.Ticks)
            {
                //TODO: Client Timeout. Disconnect Player
            }
            else
            {
                //Keep Alive Response came within time
                Console.WriteLine("Timeout Check is okey.");
            }
        }
    }
}