﻿using System;
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
            Register(PacketType.KeepAlive, 9, 0, ReadKeepAlive);
            Register(PacketType.Pattern, 25, 0, ReadPattern);
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

        public static void ReadKeepAlive(Client client, PacketReader reader)
        {
            KeepAlivePacket ap = new KeepAlivePacket();
            ap.Read(reader);

            if (!reader.Failed)
            {
                Client.HandleKeepAlive(client, ap);
            }
        }

        public static void ReadPattern(Client client, PacketReader reader)
        {
            PatternPacket pp = new PatternPacket();
            pp.Read(reader);

            if (!reader.Failed)
            {
                Client.HandlePatternPacket(client, pp);
            }
        }
    }
}