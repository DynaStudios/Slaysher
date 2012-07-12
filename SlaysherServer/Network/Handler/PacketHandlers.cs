using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;
using SlaysherServer.Game.Models;

namespace SlaysherServer.Network.Handler
{
    public class PacketHandlers
    {
        private static PacketHandler[] m_Handlers;

        public static PacketHandler[] Handlers
        {
            get { return m_Handlers; }
        }

        static PacketHandlers()
        {
            m_Handlers = new PacketHandler[0x100];

            Register(PacketType.Handshake, 0, 3, ReadHandshake);
            Register(PacketType.KeepAlive, 9, 0, ReadKeepAlive);
        }

        public static void Register(PacketType packetID, int length, int minimumLength, OnPacketReceive onReceive)
        {
            m_Handlers[(byte)packetID] = new PacketHandler(packetID, length, minimumLength, onReceive);
        }

        public static PacketHandler GetHandler(PacketType packetID)
        {
            return m_Handlers[(byte)packetID];
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

        public static void ReadKeepAlive(Client client, PacketReader reader)
        {
            KeepAlivePacket ap = new KeepAlivePacket();
            ap.Read(reader);

            if (!reader.Failed)
            {
                Client.HandleKeepAlive(client, ap);
            }
        }
    }
}