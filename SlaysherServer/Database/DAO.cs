using MySql.Data.MySqlClient;

namespace SlaysherServer.Database
{
    public class DAO
    {
        // TODO: login handling is missing! using static login information for now
        private const string ConnectionString =
            "server=direct.dyna-studios.com;"
            + "uid=slaysher;"
            + "pwd=start123;"
            + "database=slaysher;"
            + "port=3306";

        private readonly MySqlConnection _db;

        public GameObjectDAO GameObjectDAO { get; private set; }

        public PatternTypeDAO PatternTypeDAO { get; private set; }

        public DAO()
        {
            _db = new MySqlConnection(ConnectionString);
            _db.Open();

            GameObjectDAO = new GameObjectDAO(_db);
            PatternTypeDAO = new PatternTypeDAO(_db);
        }
    }
}