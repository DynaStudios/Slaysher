namespace SlaysherServer.Game
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public int Health { get; set; }

        public WorldPosition Position { get; set; }
    }
}