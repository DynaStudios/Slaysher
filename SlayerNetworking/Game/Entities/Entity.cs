using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Game.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public int Health { get; set; }

        public WorldPosition Position { get; set; }
    }
}