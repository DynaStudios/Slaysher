using System;
using System.Collections.Generic;
using System.Linq;

using SlaysherServer.Database;

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

    public class PatternGenerator
    {
        private DAO _dao;
        private List<PatternType> _types;

        public PatternGenerator(DAO dao)
        {
            _dao = dao;
            _types = _dao.PatternTypeDAO.GetAllPatternTypes();
        }

        public List<Pattern> GetPatterns()
        {
#if DEBUG
            Console.WriteLine("stating pattern generation");
#endif
            if (_types.Count <= 0)
            {
                return new List<Pattern>();
            }

            var referencing = new List<List<Pattern>>();
            var ret = new List<Pattern>();
            Random rnd = new Random();
            int xMax = rnd.Next(4) + 3;
            int yMax = rnd.Next(4) + 3;

            for (int yi = 0; yi < yMax; ++yi)
            {
                referencing.Add(new List<Pattern>());

                for (int xi = 0; xi < xMax; ++xi)
                {
                    List<PatternType> query = _types;
                    if (xi > 0)
                    {
                        int typeWest = referencing[yi][xi - 1].Type.East;
                        query = new List<PatternType>(
                            from t
                            in query
                            where t.West == typeWest
                            select t);
                    }
                    if (yi > 0)
                    {
                        int typeNort = referencing[yi - 1][xi].Type.South;
                        query = new List<PatternType>(
                            from t
                            in query
                            where t.North == typeNort
                            select t);
                    }

                    PatternType type;
                    if (query.Count == 0)
                    {
                        // FIXME: this is a fallback but should never be used as it generates
                        // not releasable stuff
                        type = _types[rnd.Next(query.Count)];
                    }
                    else
                    {
                        type = query[rnd.Next(query.Count)];
                    }

                    Pattern pattern = new Pattern()
                    {
                        Id = yi * xMax + xi,
                        Type = type,
                        X = xi * 32f,
                        Y = yi * 32f
                    };

                    ret.Add(pattern);
                    referencing[yi].Add(pattern);
                }
            }

#if DEBUG
            Console.WriteLine("pattern generation complet");
#endif
            return ret;
        }
    }
}