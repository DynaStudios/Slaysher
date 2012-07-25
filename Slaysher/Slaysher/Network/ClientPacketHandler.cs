using SlaysherNetworking.Packets;
using SlaysherNetworking.Packets.Utils;

namespace Slaysher.Network
{
    public delegate void OnPacketReceive(Client client, PacketReader pvSrc);

    public class ClientPacketHandler
    {
        private readonly int _length;
        private readonly int _minimumLength;
        private readonly OnPacketReceive _onReceive;
        private readonly PacketType _packetId;

        public PacketType PacketId
        {
            get { return _packetId; }
        }

        public int Length
        {
            get { return _length; }
        }

        public int MinimumLength
        {
            get { return _minimumLength; }
        }

        public OnPacketReceive OnReceive
        {
            get { return _onReceive; }
        }

        public ClientPacketHandler(PacketType type, int length, int minimumLength, OnPacketReceive onReceive)
        {
            _packetId = type;
            _length = length;
            _minimumLength = minimumLength;
            _onReceive = onReceive;
        }
    }
}