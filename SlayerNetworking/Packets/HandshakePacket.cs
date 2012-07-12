using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class HandshakePacket : Packet
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public HandshakePacket() { }

        public HandshakePacket(string username)
        {
            Username = username;
        }

        public override void Read(PacketReader reader)
        {
            Username = reader.ReadString16(16);
            Password = reader.ReadString8(10);
        }

        public override void Write()
        {
            SetCapacity(6, Username, Password);
            Writer.Write(Username);
            Writer.Write8(Password);
        }
    }
}