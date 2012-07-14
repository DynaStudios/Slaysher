using System.Collections.Generic;

using MySql.Data.MySqlClient;

using SlaysherNetworking.Game.World.Objects;
using SlaysherServer.Game;

namespace SlaysherServer.Database
{
    // TODO: blacklists are ignored atm
    public class PatternTypeDAO
    {
        private MySqlConnection _db;
        private MySqlCommand _allPatternTypes = null;

        internal PatternTypeDAO (MySqlConnection db)
        {
            _db = db;
        }

        public List<PatternType> GetAllPatternTypes()
        {
            if (_allPatternTypes == null)
            {
                _allPatternTypes = new MySqlCommand(
                        "SELECT id"
                        + " FROM patterntype",
                        _db);
            }

            MySqlDataReader reader = _allPatternTypes.ExecuteReader();
            List<PatternType> patternTypes = new List<PatternType>();

            while (reader.Read())
            {
                PatternType patternType = new PatternType();
                patternType._dbId = (int)reader["id"];
                patternTypes.Add(patternType);
            }

            return patternTypes;
        }
    }
}
