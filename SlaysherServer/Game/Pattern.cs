namespace SlaysherServer.Game
{
    // geographical types are:
    // invalied = 0
    // dirt = 1
    // grass = 2
    // watter = 3
    // sand (hot desert) = 4
    // ice (cold desert) = 5
    public class PatternType
    {
        internal int _dbId;
        public int North;
        public int South;
        public int West;
        public int East;
        public int TextureId;
    }

    public class Pattern
    {
        public PatternType Type { get; set; }

        public int Id { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
    }
}