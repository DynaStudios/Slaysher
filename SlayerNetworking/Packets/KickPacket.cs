using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class KickPacket : Packet
    {
        public string Message;

        public override void Read(PacketReader reader)
        {
            Message = reader.ReadString8(1024);
        }

        public override void Write()
        {
            SetCapacity();
            Writer.Write8(Message);
        }
    }
}