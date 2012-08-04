using System;

using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Game.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public float ModelScaling { get; set; }

        public int ModelId { get; set; }

        public int TextureId { get; set; }

        public int Health { get; set; }

        // Speed in meter per second
        public float Speed
        {
            get { return SpeedMeeterPerMillisecond * 1000; }
            set { SpeedMeeterPerMillisecond = value / 1000; }
        }
        public float SpeedMeeterPerMillisecond { get; set; }

        public virtual WorldPosition Position { get; set; }

        // position where a movement has started
        private WorldPosition _startPosition = null;

        protected TimeSpan? _movemetStarted = null;

        private float _direction;
        public float Direction { get { return _direction; } }

        public Entity()
        {
            Speed = 2.0f;
        }

        public void Move(TimeSpan totalTime, float direction)
        {
            ExecuteMovement(totalTime);
            // using now position for the next movement
            _startPosition = new WorldPosition(Position);
            _movemetStarted = totalTime;
            Turn(direction);
        }

        public void Turn(float direction)
        {
            // XXX: smothered turnning would be nice
            this._direction = direction;
        }

        public void StopMoving(TimeSpan totalTime)
        {
            // direction should stay as it is
            ExecuteMovement(totalTime);
            _startPosition = null;
            _movemetStarted = null;
        }

        public void ExecuteMovement(TimeSpan current)
        {
            if (_movemetStarted == null || _startPosition == null)
            {
                return;
            }
            TimeSpan movedTime = current - (TimeSpan)_movemetStarted;
            float distance = (float) (movedTime.TotalMilliseconds * SpeedMeeterPerMillisecond);

            Position.X = _startPosition.X + (float)(Math.Sin(_direction * Math.PI / 180) * distance);
            Position.Y = _startPosition.Y - (float)(Math.Cos(_direction * Math.PI / 180) * distance);
        }
    }
}
