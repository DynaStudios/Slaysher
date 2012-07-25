using System.Collections.Generic;

using MySql.Data.MySqlClient;
using SlaysherServer.Game;

namespace SlaysherServer.Database
{
    public class PatternTypeDAO
    {
        private readonly MySqlConnection _db;
        private MySqlCommand _allPatternTypes;

        internal PatternTypeDAO (MySqlConnection db)
        {
            _db = db;
        }

        public List<PatternType> GetAllPatternTypes()
        {
            if (_allPatternTypes == null)
            {
                _allPatternTypes = new MySqlCommand(
                        "SELECT id, north, south, west, east, textureid"
                        + " FROM patterntype",
                        _db);
            }

            MySqlDataReader reader = _allPatternTypes.ExecuteReader();
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
}
