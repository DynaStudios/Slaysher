using System;
using System.Collections.Generic;

using Npgsql;
//using MySql.Data.MySqlClient;

using SlaysherServer.Game;

namespace SlaysherServer.Database
{
    public class PatternTypeDAO : IDisposable
    {
        private DAO dao;
        private NpgsqlConnection Db { get { return dao.DBConnection; } }
        private NpgsqlCommand _allPatternTypes;
        private NpgsqlCommand AllPatternTypes
        {
            get
            {
                if (_allPatternTypes == null)
                {
                    _allPatternTypes = new NpgsqlCommand(
                        "SELECT id, north, south, west, east, textureid"
                        + " FROM patterntype",
                        Db);
                }
                return _allPatternTypes;
            }
        }

        internal PatternTypeDAO(DAO dao)
        {
            this.dao = dao;
        }

        public List<PatternType> GetAllPatternTypes()
        {
            using (NpgsqlDataReader reader = AllPatternTypes.ExecuteReader())
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
            if (_allPatternTypes != null)
            {
                _allPatternTypes.Dispose();
            }
        }
    }
}