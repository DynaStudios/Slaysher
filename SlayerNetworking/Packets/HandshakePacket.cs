using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class HandshakePacket : Packet
    {
        public int UserId { get; set; }

        public override void Read(PacketReader reader)
        {
            UserId = reader.ReadInt();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(UserId);
        }
    }
}