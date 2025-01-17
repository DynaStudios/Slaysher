﻿using System;
using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PlayerInfoPacket : Packet
    {
        public int PlayerId { get; set; }

        public string Nickname { get; set; }

        public int Health { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        public float Speed { get; set; }

        public PlayerInfoPacket()
        {
        }

        public PlayerInfoPacket(IPlayer player)
        {
            PlayerId = player.Id;
            Nickname = player.Nickname;
            Health = player.Health;
            Speed = player.SpeedMeterPerMillisecond;
        }

        public override void Read(PacketReader reader)
        {
            PlayerId = reader.ReadInt();
            Nickname = reader.ReadString16(9);
            Health = reader.ReadInt();
            Speed = reader.ReadFloat();
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }

        public override void Write()
        {
            SetCapacity(23, Nickname);
            Writer.Write(PlayerId);
            Writer.Write(Nickname);
            Writer.Write(Health);
            Writer.Write(Speed);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }
    }
}