using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

namespace Slaysher.Network
{
    public class PacketHandlers
    {
        private static ClientPacketHandler[] _handlers;

        public static ClientPacketHandler[] Handlers
        {
            get { return _handlers; }
        }

        static PacketHandlers()
        {
            _handlers = new ClientPacketHandler[0x100];

            Register(PacketType.Handshake, 0, 3, ReadHandshake);
        }

        public static void Register(PacketType packetID, int length, int minimumLength, OnPacketReceive onReceive)
        {
            _handlers[(byte)packetID] = new ClientPacketHandler(packetID, length, minimumLength, onReceive);
        }

        public static ClientPacketHandler GetHandler(PacketType packetID)
        {
            return _handlers[(byte)packetID];
        }

        public static void ReadHandshake(Client client, PacketReader reader)
        {
            HandshakePacket hp = new HandshakePacket();
            hp.Read(reader);

            if (!reader.Failed)
            {
                Client.HandleHandshake(client, hp);
            }
        }
    }
}