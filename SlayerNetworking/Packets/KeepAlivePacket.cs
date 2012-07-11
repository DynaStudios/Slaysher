using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class KeepAlivePacket : Packet
    {
        public long timeStamp;

        public override void Read(PacketReader reader)
        {
            timeStamp = reader.ReadLong();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(timeStamp);
        }
    }
}