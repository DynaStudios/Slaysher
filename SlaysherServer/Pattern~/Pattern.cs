using System;
using System.Collections.Generic;

using SlaysherServer.Database;

namespace SlaysherServer.Pattern
{
    internal class PatternType
    {
        internal int _dbId;
        public string _name;
    }

    internal class Pattern
    {
        public PatternType Type  { get; set; }
        public int TextureId { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
    }

    internal class PatternGenerator {
        private DAO _dao;

        public PatternGenerator(DAO dao)
        {
            _dao = dao;
        }

        public List<Pattern> getPatterns()
        {
            List <Pattern> ret = new List<Pattern>();

            List<PatternType> types = _dao.PatternTypeDAO.GetAllPatternTypes();
            Random rnd = new Random();
            int countPattern = rnd.Next(10) + 10;
            for (int i = 0; i < countPattern; ++i)
            {

            }
        }
    }
}
