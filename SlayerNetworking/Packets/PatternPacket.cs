using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PatternPacket : Packet
    {
        public int PatternId { get; set; }

        public int TextureId { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        protected override int Length
        {
            get
            {
                return 25;
            }
        }

        public override void Read(PacketReader reader)
        {
            PatternId = reader.ReadInt();
            TextureId = reader.ReadInt();
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(PatternId);
            Writer.Write(TextureId);
            Writer.Write(X);
            Writer.Write(Y);
        }
    }
}