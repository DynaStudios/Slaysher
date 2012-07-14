using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PatternPacket : Packet
    {
        public int PatternID { get; set; }

        public int TextureID { get; set; }

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
            PatternID = reader.ReadInt();
            TextureID = reader.ReadInt();
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(PatternID);
            Writer.Write(TextureID);
            Writer.Write(X);
            Writer.Write(Y);
        }
    }
}