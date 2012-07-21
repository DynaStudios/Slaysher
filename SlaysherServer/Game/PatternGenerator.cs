using System;
using System.Collections.Generic;
using System.Linq;

using SlaysherServer.Database;

namespace SlaysherServer.Game
{
    public class PatternGenerator
    {
        private DAO _dao;
        private List<PatternType> _types;
        private Random _rnd = new Random();

        public PatternGenerator(DAO dao)
        {
            _dao = dao;
            _types = _dao.PatternTypeDAO.GetAllPatternTypes();
        }

        private IEnumerable<PatternType> filterForWestBorder(IEnumerable<PatternType> src, int borderType)
        {
            return from t
                in src
                where t.West == borderType
                select t;
        }

        private IEnumerable<PatternType> filterForNordBorder(IEnumerable<PatternType> src, int borderType)
        {
            return new List<PatternType>(
                from t
                in src
                where t.North == borderType
                select t);
        }

        private List<PatternType> getFilteredPatternType(List<List<Pattern>> referenc, int x, int y)
        {
            IEnumerable<PatternType> filterQuery = _types;
            if (x > 0)
            {
                filterQuery = filterForWestBorder(
                    filterQuery,
                    referenc[y][x - 1].Type.East);  // the east border of the west pattern
            }
            if (y > 0)
            {
                filterQuery = filterForNordBorder(
                    filterQuery,
                    referenc[y - 1][x].Type.South);
            }
            return new List<PatternType>(filterQuery);
        }

        // XXX: splitting method in smaller ones for futer maintain
        public List<Pattern> GetPatterns()
        {
#if DEBUG
            Console.WriteLine("stating pattern generation");
#endif
            if (_types.Count <= 0)
            {
                return new List<Pattern>();
            }

            var referenc = new List<List<Pattern>>();
            var ret = new List<Pattern>();
            Random rnd = new Random();
            int xMax = rnd.Next(4) + 3;
            int yMax = rnd.Next(4) + 3;

            for (int yi = 0; yi < yMax; ++yi)
            {
                referenc.Add(new List<Pattern>());

                for (int xi = 0; xi < xMax; ++xi)
                {

                    List<PatternType> query = getFilteredPatternType(referenc, xi, yi);

                    PatternType type;
                    if (query.Count == 0)
                    {
                        if (xi > 0)
                        {
                            // this could squru the server
                            ret.RemoveAt(ret.Count - 1);
                            List<Pattern> tmp = referenc[yi];
                            tmp.RemoveAt(tmp.Count - 1);
                            xi -= 2;
                            continue;
                        }
                        // FIXME: this is a fallback but should never be used as it generates
                        // broken game maps
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
                    referenc[yi].Add(pattern);
                }
            }

#if DEBUG
            Console.WriteLine("pattern generation complet");
#endif
            return ret;
        }
    }
}
