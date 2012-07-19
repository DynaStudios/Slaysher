using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PlayerInfoPacket : Packet
    {
        public int PlayerId { get; set; }

        public string Nickname { get; set; }

        public int Health { get; set; }

        public override void Read(PacketReader reader)
        {
            PlayerId = reader.ReadInt();
            Nickname = reader.ReadString8(12);
            Health = reader.ReadInt();
        }

        public override void Write()
        {
            SetCapacity(9, Nickname);
            Writer.Write(PlayerId);
            Writer.Write8(Nickname);
            Writer.Write(Health);
        }
    }
}