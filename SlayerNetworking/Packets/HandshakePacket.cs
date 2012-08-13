using System;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class HandshakePacket : Packet
    {
        public string Username { get; set; }

        public HandshakePacket()
        {
        }

        public HandshakePacket(string username)
        {
            Username = username;
        }

        public override void Read(PacketReader reader)
        {
            Username = reader.ReadString16(64);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }

        public override void Write()
        {
            SetCapacity(3, Username);
            Writer.Write(Username);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }
    }
}