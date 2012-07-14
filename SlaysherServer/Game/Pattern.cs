using System;
using System.Collections.Generic;

using SlaysherServer.Database;

namespace SlaysherServer.Game
{
    public class PatternType
    {
        internal int _dbId;
        public string _name;
    }

    public class Pattern
    {
        public PatternType Type  { get; set; }
        public int Id { get; set; }
        public int TextureId { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
    }

    public class PatternGenerator {
        private DAO _dao;

        public PatternGenerator(DAO dao)
        {
            _dao = dao;
        }

        public List<Pattern> GetPatterns()
        {
            List <Pattern> ret = new List<Pattern>();

            List<PatternType> types = _dao.PatternTypeDAO.GetAllPatternTypes();
            Random rnd = new Random();
            int xMax = rnd.Next(5) + 5;
            int yMax = rnd.Next(5) + 5;

            for (int yi=0; yi < yMax; ++yi) {
                for (int xi = 0; xi < xMax; ++xi)
                {
                    PatternType type = types[rnd.Next(types.Count)];

                    Pattern pattern = new Pattern()
                    {
                        TextureId = rnd.Next(10),
                        Type = type,
                        X = xi,
                        Y = yi
                    };

                    ret.Add(pattern);
                }
            }

            return ret;
        }
    }
}
