using System;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class EntityDespawnPacket : Packet
    {
        public int EntityId { get; set; }

        protected override int Length
        {
            get { return 5; }
        }

        public override void Read(PacketReader reader)
        {
            EntityId = reader.ReadInt();
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write(EntityId);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }
    }
}