using System;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class KeepAlivePacket : Packet
    {
        public long TimeStamp { get; set; }

        protected override int Length
        {
            get { return 9; }
        }

        public override void Read(PacketReader reader)
        {
            TimeStamp = reader.ReadLong();
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(TimeStamp);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }
    }
}