using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PlayerPositionPacket : Packet
    {
        public int PlayerId { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        protected override int Length
        {
            get { return 17; }
        }

        public PlayerPositionPacket()
        {
        }

        public PlayerPositionPacket(Player player)
        {
            PlayerId = player.Id;
            X = player.Position.X;
            Y = player.Position.Y;
            Z = 0f;
        }

        public override void Read(PacketReader reader)
        {
            PlayerId = reader.ReadInt();
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
            Z = reader.ReadFloat();
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(PlayerId);
            Writer.Write(X);
            Writer.Write(Y);
            Writer.Write(Z);
        }
    }
}