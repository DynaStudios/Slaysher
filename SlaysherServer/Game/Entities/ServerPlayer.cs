using System;

using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets;

using SlaysherServer.Network;

namespace SlaysherServer.Game.Entities
{
    public class ServerPlayer : Player
    {
        public MovePacket CreatePreperedMovePacket(TimeSpan totalTime)
        {
            if (ExecutePreparedMove(totalTime))
            {
                MovePacket mp = new MovePacket
                {
                    EntetyId = Id,
                    Direction = Direction,
                    Position = Position,
                    Speed = IsMoving ? Speed : 0.0f,
                };

                return mp;
            }
            return null;
        }
    }
}
