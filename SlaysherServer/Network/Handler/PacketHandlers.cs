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
        }

        public static void Register(PacketType packetID, int length, int minimumLength, OnPacketReceive onReceive)
        {
            m_Handlers[(byte)packetID] = new PacketHandler(packetID, length, minimumLength, onReceive);

            Register(PacketType.Handshake, 4, 0, ReadHandshake);
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
            }
        }
    }
}