using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

using SlaysherServer.Database;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public int TimesEnqueuedForRecv;
        private readonly object _queueSwapLock = new object();

        private void RecvStart()
        {
            if (!Running)
            {
                DisposeRecvSystem();
                return;
            }

            if (!_socket.Connected)
            {
                Stop();
                return;
            }

            try
            {
                bool pending = _socket.ReceiveAsync(_recvSocketEvent);

                if (!pending)
                    RecvCompleted(null, _recvSocketEvent);
            }
            catch (Exception)
            {
                //TODO:
                //Yo Patrick you should really catch this shit!
            }
        }

        private void RecvCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (!Running)
                DisposeRecvSystem();
            else if (e.SocketError != SocketError.Success || e.BytesTransferred == 0)
            {
                _nextActivityCheck = DateTime.MinValue;
            }
            else
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

            Server.NetworkSignal.Set();

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

        private void SendPattern()
        {
            foreach (Pattern pattern in Server.World.Patterns)
            {
                PatternPacket packet = new PatternPacket
                    {
                        PatternId = pattern.Id,
                        TextureId = pattern.Type.TextureId,
                        X = pattern.X,
                        Y = pattern.Y
                    };

                SendSyncPacket(packet);
                
            }
        }

        public void HandleHandshake(HandshakePacket packet)
        {
            if (packet.Username != null && !IsLoggingIn)
            {
                IsLoggingIn = true;
                // check for baned users
                // if (banlist.Contains(client)) {
                //    client.SendPacket(new KickPacket() { message = "You'r BANNED!!!! Get lost!" };
                //    return;
                // }

                Console.WriteLine("Received Login for User " + packet.Username);

                Console.WriteLine("Send Handshake back!");
                SendPacket(new HandshakePacket(packet.Username));

                Console.WriteLine("Send Player Information");
                LoadPlayer();
                SendPlayerInfo();

                Console.WriteLine("Send Pattern");
                SendPattern();

                LastSendKeepAliveStamp = DateTime.Now.Ticks;

                Console.WriteLine("Finished Init. Send KeepAlive");
                KeepAlivePacket keepAlive = new KeepAlivePacket {TimeStamp = LastSendKeepAliveStamp};
                SendPacket(keepAlive);
            }
        }

        public void SendPlayerInfo()
        {
            SendPlayerInfo(Player);
        }

        private void SendPlayerInfo(IPlayer player)
        {
            SendPacket(new PlayerInfoPacket(player));
            //SendPacket(new PlayerPositionPacket(Player));
        }

        private void LoadPlayer()
        {
            Player = Server.DAO.Player.getForClient(DbId);
            Player.Id = ClientId;

            IEnumerable<Client> clients = Server.GetNearbyPlayers(Player.Position);
            InformClients(clients);
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

        public static void HandleMovePacket(Client client, MovePacket mp)
        {
            client.Player.PrepareToMove(mp.Direction, mp.Speed);
        }
    }
}