﻿using System;
using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

namespace Slaysher.Network
{
    public class PacketHandlers
    {
        private static readonly ClientPacketHandler[] _handlers;

        public static ClientPacketHandler[] Handlers
        {
            get { return _handlers; }
        }

        static PacketHandlers()
        {
            _handlers = new ClientPacketHandler[0x100];

            Register(PacketType.Handshake, 0, 3, ReadHandshake);
            Register(PacketType.KeepAlive, 9, 0, ReadKeepAlive);
            Register(PacketType.Pattern, 17, 0, ReadPattern);
            Register(PacketType.EntitySpawn, 0, 17, ReadEntitySpawn);
            Register(PacketType.EntityDespawn, 5, 0, ReadEntityDespawn);
            Register(PacketType.PlayerInfo, 0, 23, ReadPlayerInfo);
            Register(PacketType.PlayerPosition, 17, 0, ReadPlayerPosition);
            Register(PacketType.Movement, 21, 0, ReadMovement);
        }

        public static void Register(PacketType packetId, int length, int minimumLength, OnPacketReceive onReceive)
        {
            _handlers[(byte) packetId] = new ClientPacketHandler(packetId, length, minimumLength, onReceive);
        }

        public static ClientPacketHandler GetHandler(PacketType packetId)
        {
            return _handlers[(byte) packetId];
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

        public static void ReadEntitySpawn(Client client, PacketReader reader)
        {
            EntitySpawnPacket esp = new EntitySpawnPacket();
            esp.Read(reader);

            if (!reader.Failed)
            {
                client.HandleEntitySpawn(esp);
            }
        }

        public static void ReadEntityDespawn(Client client, PacketReader reader)
        {
            EntityDespawnPacket edp = new EntityDespawnPacket();
            edp.Read(reader);

            if (!reader.Failed)
            {
                client.HandleEntityDespawn(edp);
            }
        }

        public static void ReadPlayerInfo(Client client, PacketReader reader)
        {
            PlayerInfoPacket pip = new PlayerInfoPacket();
            Console.WriteLine("Player Info Packet size is {0}", reader.Size);
            pip.Read(reader);

            if (!reader.Failed)
            {
                client.HandlePlayerInfo(pip);
            }
        }

        public static void ReadPlayerPosition(Client client, PacketReader reader)
        {
            PlayerPositionPacket ppp = new PlayerPositionPacket();
            ppp.Read(reader);

            if (!reader.Failed)
            {
                client.HandlePlayerPosition(ppp);
            }
        }

        public static void ReadMovement(Client client, PacketReader reader)
        {
            MovePacket mp = new MovePacket();
            mp.Read(reader);

            if (!reader.Failed)
            {
                Console.WriteLine("Received Move Packet");
                client.Move(
                    mp.EntityId,
                    mp.Position,
                    mp.Direction,
                    mp.Speed);
            }
        }
    }
}