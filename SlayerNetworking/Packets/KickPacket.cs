using System;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class KickPacket : Packet
    {
        public string Message;

        public override void Read(PacketReader reader)
        {
            Message = reader.ReadString8(1024);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write8(Message);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }
    }
}