using System;

using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets;

using SlaysherServer.Network;

namespace SlaysherServer.Game.Entities
{
    public class ServerPlayer : ServerEntity, IPlayer
    {
        public string Nickname { get; set; }

        public MovePacket CreatePreparedMovePacket(TimeSpan totalTime)
        {
            if (this.ExecutePreparedMove(totalTime))
            {
                MovePacket mp = new MovePacket
                {
                    EntityId = Id,
                    Direction = Direction,
                    Position = Position,
                    Speed = IsMoving ? Speed : 0.0f,
                };

                return mp;
            }
            return null;
        }

        public override void Tick(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}
