using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class EntitySpawnPacket : Packet
    {
        public int EntityId { get; set; }

        public string Nickname { get; set; }

        public int Health { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public override void Read(PacketReader reader)
        {
            EntityId = reader.ReadInt();
            Nickname = reader.ReadString8(12);
            Health = reader.ReadInt();
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
        }

        public override void Write()
        {
            SetCapacity(17, Nickname);
            Writer.Write(EntityId);
            Writer.Write8(Nickname);
            Writer.Write(Health);
            Writer.Write(X);
            Writer.Write(Y);
        }
    }
}