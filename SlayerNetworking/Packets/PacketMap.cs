using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlaysherNetworking.Packets
{
    public static class PacketMap
    {
        public static void Initialize()
        {
            foreach (KeyValuePair<Type, PacketType> kvp in _map)
                _concurrentMap.TryAdd(kvp.Key, kvp.Value);

            // Lets free some memory
            _map = null;
        }

        private static Dictionary<Type, PacketType> _map = new Dictionary<Type, PacketType>
        {
            { typeof(HandshakePacket), PacketType.Handshake },
            { typeof(KickPacket), PacketType.Kick },
            { typeof(KeepAlivePacket), PacketType.KeepAlive },
            { typeof(PlayerPositionPacket), PacketType.PlayerPosition },
            { typeof(PatternPacket), PacketType.Pattern},
            { typeof(EntitySpawnPacket), PacketType.EntitySpawn},
            { typeof(EntityDespawnPacket), PacketType.EntityDespawn}
        };

        private readonly static ConcurrentDictionary<Type, PacketType> _concurrentMap = new ConcurrentDictionary<Type, PacketType>();

        public static ConcurrentDictionary<Type, PacketType> Map { get { return _concurrentMap; } }

        public static PacketType GetPacketType(Type type)
        {
            PacketType packetType;
            if (_concurrentMap.TryGetValue(type, out packetType))
                return packetType;

            throw new KeyNotFoundException();
        }
    }
}