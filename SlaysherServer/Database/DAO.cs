using System.Collections.Generic;

using MySql.Data;
using MySql.Data.MySqlClient;

using SlaysherNetworking.Game.World.Objects;

using SlaysherServer.Game;

namespace SlaysherServer.Database
{
    public class DAO
    {
        // TODO: login handling is missing! using static login information for now
        private const string _connectionString = "server=localhost;user=root;database=slaysher;port=3306";
        private MySqlConnection _db;

        public GameObjectDAO GameObjectDAO { get; private set; }
        public PatternTypeDAO PatternTypeDAO { get; private set; }

        public DAO()
        {
            _db = new MySqlConnection(_connectionString);
            GameObjectDAO = new GameObjectDAO(_db);
            PatternTypeDAO = new PatternTypeDAO(_db);
        }

    }
}
