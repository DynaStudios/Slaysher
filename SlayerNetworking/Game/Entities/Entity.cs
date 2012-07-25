using System;

using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Game.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        public int TextureId { get; set; }

        public int Health { get; set; }

        public float Speed { get; set; }

        public WorldPosition Position { get; set; }

        protected DateTime _movemetStarted = new DateTime(0);

        protected float direction;

        public void Move(float direction)
        {
            DateTime now = DateTime.Now;
            executeMovement(now);
        }

        private void executeMovement(DateTime movementEndTime)
        {
            if (_movemetStarted.Ticks == 0)
            {
                return;
            }
            TimeSpan movedTime = movementEndTime - _movemetStarted;
            float distance = movedTime.Ticks * Speed;


        }
    }
}
