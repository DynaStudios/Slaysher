using System;

using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Game.World;

namespace Slaysher.Game.Entities
{
    public abstract class ClientEntity : IEntity
    {
        public int Id { get; set; }

        public float ModelScaling { get; set; }

        public int ModelId { get; set; }

        public int TextureId { get; set; }

        public int Health { get; set; }

        public float SpeedMeeterPerMillisecond { get; set; }
        public float Speed
        {
            get { return this.GetSpeed(); }
            set { this.SetSeed(value); }
        }

        public WorldPosition Position { get; set; }

        // position where a movement has started
        // should be null at init
        public WorldPosition _startPosition { get; set; }

        // should be null at init
        public TimeSpan? _movemetStarted { get; set; }

        public float Direction { get; set; }

        public float? _preparedDirection { get; set; }
        public float? _preparedSpeed { get; set; }

        public abstract void Tick(TimeSpan timeSpan);
    }
}
