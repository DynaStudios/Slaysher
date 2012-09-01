using Npgsql;
using System;

using System.Data.Common;
//using MySql.Data.MySqlClient;

namespace SlaysherServer.Database
{
    public class DAO : IDisposable
    {
        // TODO: login handling is missing! using static login information for now
        private const string ConnectionString =
            "server=direct.dyna-studios.com;"
            + "User ID=postgres;"
            + "Password=start123;"
			+ "Database=slaysher;";

        public DbConnection DBConnection { get; private set; }

        public GameObjectDAO GameObjectDAO { get; private set; }

        public PatternTypeDAO PatternTypeDAO { get; private set; }

        public PlayerDAO Player { get; private set; }

        public DAO()
        {
            DBConnection = new NpgsqlConnection(ConnectionString);
            DBConnection.Open();

            InitSubDAOs();
        }

        public DAO(DbConnection connection)
        {
            DBConnection = connection;

            InitSubDAOs();
        }

        private void InitSubDAOs()
        {
            GameObjectDAO = new GameObjectDAO(this);
            PatternTypeDAO = new PatternTypeDAO(this);
            Player = new PlayerDAO(this);
        }

        public void Dispose()
        {
            GameObjectDAO.Dispose();
            PatternTypeDAO.Dispose();
            Player.Dispose();
            DBConnection.Dispose();
        }
    }
}