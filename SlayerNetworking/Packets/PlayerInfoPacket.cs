using System;
using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public class PlayerInfoPacket : Packet
    {
        public int PlayerId { get; set; }

        public string Nickname { get; set; }

        public int Health { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        public PlayerInfoPacket()
        {
        }

        public PlayerInfoPacket(IPlayer player)
        {
            PlayerId = player.Id;
            Nickname = player.Nickname;
            Health = player.Health;
        }

        public override void Read(PacketReader reader)
        {
            PlayerId = reader.ReadInt();
            Nickname = reader.ReadString16(12);
            Health = reader.ReadInt();
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }

        public override void Write()
        {
            SetCapacity(17, Nickname);
            Writer.Write(PlayerId);
            Writer.Write(Nickname);
            Writer.Write(Health);
#if DEBUG
            Console.WriteLine(ToString());
#endif
        }
    }
}