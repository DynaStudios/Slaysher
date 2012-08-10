using SlaysherNetworking.Packets.Utils;
using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Packets
{
    public class MovePacket : Packet
    {
        public WorldPosition Position { get; set; }
        public int EntityId { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }
        protected override int Length { get { return 21; } }

        public MovePacket()
        {
            Position = new WorldPosition();
        }

        public override void Read(PacketReader reader) {
            EntityId = reader.ReadInt();
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
            Writer.Write(EntityId);
            Writer.Write(Position.X);
            Writer.Write(Position.Y);
            Writer.Write(Direction);
            Writer.Write(Speed);
        }
    }
}
