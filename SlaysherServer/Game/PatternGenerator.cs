using System;
using System.Collections.Generic;
using System.Linq;

using SlaysherServer.Database;

namespace SlaysherServer.Game
{
    public class PatternGenerator
    {
        private DAO _dao;
        private PatternTemplate[,] _tempates = null;
        private List<PatternType> _types;
        private Random _rnd = new Random();

        public PatternGenerator(DAO dao)
        {
            _dao = dao;
            _types = _dao.PatternTypeDAO.GetAllPatternTypes();
        }

        private void generateTempatesArray()
        {
            if (_tempates == null)
            {
                int x = _rnd.Next(3, 7);
                int y = _rnd.Next(3, 7);

                _tempates = new PatternTemplate[y, x];
            }
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

        private List<PatternType> getFilteredPatternType(int x, int y)
        {
            IEnumerable<PatternType> filterQuery = _types;
            if (x > 0)
            {
                filterQuery = filterForEastBorder(
                    filterQuery,
                    referencing[y][x - 1].Type.East);  // the east border of the west pattern
            }
            if (yi > 0)
            {
                filterQuery = filterForNordBorder(
                    filterQuery,
                    referencing[y - 1][x].Type.South);
            }
            return new List(filterQuery);
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

                    List<PatternType> query = getFilteredPatternType(xi, yi);

                    PatternType type;
                    if (query.Count == 0)
                    {
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
