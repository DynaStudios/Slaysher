using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class KickPacket : Packet
    {
        public string message;

        public override void Read(PacketReader reader)
        {
            message = reader.ReadString8(1024);
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write8(message);
        }
    }
}
