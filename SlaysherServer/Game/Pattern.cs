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
        internal int _dbId = 0;
        public int North = 0;
        public int South = 0;
        public int West = 0;
        public int East = 0;
        public int TextureId = 0;
    }

    public class Pattern
    {
        public PatternType Type { get; set; }

        public int Id { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
    }
}