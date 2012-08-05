using SlaysherNetworking.Packets.Utils;
using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Packets
{
    public class MovePacket : Packet
    {
        public WorldPosition Position { get; set; }
        public int EntetyId { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }
        protected override int Length { get { return 37; } }

        public MovePacket()
        {
            Position = new WorldPosition();
        }

        public override void Read(PacketReader reader) {
            EntetyId = reader.ReadInt();
            Position = new WorldPosition
            {
                X = reader.ReadFloat(),
                Y = reader.ReadFloat()
            };
            Direction = reader.ReadFloat();
            Speed = reader.ReadFloat();
        }

        public override void Write() {
            SetCapacity();
            Writer.Write(EntetyId);
            Writer.Write(Position.X);
            Writer.Write(Position.Y);
            Writer.Write(Direction);
            Writer.Write(Speed);
        }
    }
}
