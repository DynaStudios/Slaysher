using Npgsql;

namespace SlaysherServer.Database
{
    public class DAO
    {
        // TODO: login handling is missing! using static login information for now
        private const string ConnectionString =
            "server=direct.dyna-studios.com;"
            + "User ID=postgres;"
            + "Password=start123;"
			+ "Database=slaysher;";

		private readonly NpgsqlConnection _dbcon;

        public GameObjectDAO GameObjectDAO { get; private set; }

        public PatternTypeDAO PatternTypeDAO { get; private set; }

        public DAO()
        {
            _dbcon = new NpgsqlConnection(ConnectionString);
            _dbcon.Open();

            GameObjectDAO = new GameObjectDAO(_dbcon);
            PatternTypeDAO = new PatternTypeDAO(_dbcon);
        }
    }
}