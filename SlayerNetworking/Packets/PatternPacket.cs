using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PatternPacket : Packet
    {
        public int patternId;
        public int textureId;
        public float x;
        public float y;

        public override void Read(PacketReader reader)
        {
            patternId = reader.ReadInt();
            textureId = reader.ReadInt();
            x = reader.ReadFloat();
            y = reader.ReadFloat();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(patternId);
            Writer.Write(textureId);
            Writer.Write(x);
            Writer.Write(y);
        }
    }
}
