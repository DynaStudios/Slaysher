namespace SlaysherNetworking.Packets
{
    public enum PacketType : byte
    {
        Handshake = 0x00,
        Kick = 0x01,
        KeepAlive = 0x02,
        PlayerPosition = 0x03,
        Pattern = 0x04,
        EntitySpawn = 0x05,
        EntityDespawn = 0x06,
        PlayerInfo = 0x07,
        Movement = 0x08
    }
}