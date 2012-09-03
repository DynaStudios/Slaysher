using Npgsql;

using SlaysherNetworking.Plugin;

using SlaysherServer;
using SlaysherServer.Database;

namespace PostgreSqlPlugin
{
    public class MainPlugin : IPlugin<Server>
    {
        // TODO: login handling is missing! using static login information for now
        private const string ConnectionString =
            "server=direct.dyna-studios.com;"
            + "User ID=postgres;"
            + "Password=start123;"
            + "Database=slaysher;";

        public void Init(Server server)
        {
            NpgsqlConnection DBConnection = new NpgsqlConnection(ConnectionString);
            DBConnection.Open();
            DAO dao = new DAO(DBConnection);
            server.DAO = dao;
        }
    }
}
