using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;
using SlaysherServer.Game.Models;

namespace SlaysherServer.Network.Handler
{
    public class PacketHandlers
    {
        private static readonly PacketHandler[] MHandlers;

        public static PacketHandler[] Handlers
        {
            get { return MHandlers; }
        }

        static PacketHandlers()
        {
            MHandlers = new PacketHandler[0x100];

            Register(PacketType.Handshake, 0, 3, ReadHandshake);
            Register(PacketType.KeepAlive, 9, 0, ReadKeepAlive);
            Register(PacketType.Movement, 21, 0, ReadMovement);
        }

        public static void Register(PacketType packetId, int length, int minimumLength, OnPacketReceive onReceive)
        {
            MHandlers[(byte) packetId] = new PacketHandler(packetId, length, minimumLength, onReceive);
        }

        public static PacketHandler GetHandler(PacketType packetId)
        {
            return MHandlers[(byte) packetId];
        }

        public static void ReadHandshake(Client client, PacketReader reader)
        {
            HandshakePacket hp = new HandshakePacket();
            hp.Read(reader);

            if (!reader.Failed)
            {
                client.HandleHandshake(hp);
            }
        }

        public static void ReadKeepAlive(Client client, PacketReader reader)
        {
            KeepAlivePacket ap = new KeepAlivePacket();
            ap.Read(reader);

            if (!reader.Failed)
            {
                Client.HandleKeepAlive(client, ap);
            }
        }

        private static void ReadMovement(Client client, PacketReader reader)
        {
            MovePacket mp = new MovePacket();
            mp.Read(reader);

            if (!reader.Failed)
            {
                Client.HandleMovePacket(client, mp);
            }
        }
    }
}