using System.Collections.Generic;
using Npgsql;
using SlaysherServer.Game;

namespace SlaysherServer.Database
{
    public class PatternTypeDAO
    {
        private readonly NpgsqlConnection _db;
        private NpgsqlCommand _allPatternTypes;

        internal PatternTypeDAO(NpgsqlConnection db)
        {
            _db = db;
        }

        public List<PatternType> GetAllPatternTypes()
        {
            if (_allPatternTypes == null)
            {
                _allPatternTypes = new NpgsqlCommand(
                    "SELECT id, north, south, west, east, textureid"
                    + " FROM patterntype",
                    _db);
            }

            var reader = _allPatternTypes.ExecuteReader();
            List<PatternType> patternTypes = new List<PatternType>();

            while (reader.Read())
            {
                PatternType patternType = new PatternType
                    {
                        DbId = (int) reader["id"],
                        North = (int) reader["north"],
                        South = (int) reader["south"],
                        West = (int) reader["west"],
                        East = (int) reader["east"],
                        TextureId = (int) reader["textureid"]
                    };
                patternTypes.Add(patternType);
            }

            return patternTypes;
        }
    }
}