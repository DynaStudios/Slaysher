using System;
using System.Collections.Generic;
using System.Data.Common;

using SlaysherServer.Game;

namespace SlaysherServer.Database
{
    public class PatternTypeDAO : IDisposable
    {
        private DAO dao;
        private DbConnection Db { get { return dao.DBConnection; } }
        private DbCommand AllPatternTypes { get; set; }

        internal PatternTypeDAO(DAO dao)
        {
            this.dao = dao;

            AllPatternTypes = Db.CreateCommand();
            AllPatternTypes.CommandText = "SELECT id, north, south, west, east, textureid"
                                        + " FROM patterntype";
        }

        public List<PatternType> GetAllPatternTypes()
        {
            using (DbDataReader reader = AllPatternTypes.ExecuteReader())
            {
                List<PatternType> patternTypes = new List<PatternType>();

                while (reader.Read())
                {
                    PatternType patternType = new PatternType
                    {
                        DbId = (int)reader["id"],
                        North = (int)reader["north"],
                        South = (int)reader["south"],
                        West = (int)reader["west"],
                        East = (int)reader["east"],
                        TextureId = (int)reader["textureid"]
                    };
                    patternTypes.Add(patternType);
                }

                return patternTypes;
            }
        }

        public void Dispose()
        {
            if (AllPatternTypes != null)
            {
                AllPatternTypes.Dispose();
            }
        }
    }
}