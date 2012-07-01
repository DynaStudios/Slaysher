using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlaysherNetworking.Packets
{
    public enum PacketType : byte
    {
        Handshake = 0x00,
        Kick = 0x01,
        KeepAlive = 0x02,
        PlayerPosition = 0x03,
        Patern = 0x04
    }
}