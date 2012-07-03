using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PlayerPositionPacket : Packet
    {
        public float x;
        public float y;
        public float z;

        public override void Read(PacketReader reader)
        {
            x = reader.ReadFloat();
            y = reader.ReadFloat();
            z = reader.ReadFloat();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(x);
            Writer.Write(y);
            Writer.Write(z);
        }
    }
}
