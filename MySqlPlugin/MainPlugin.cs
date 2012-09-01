using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;

using SlaysherServer;
using SlaysherServer.Database;

namespace MySqlPlugin
{
    public class MainPlugin : IServerPlugin
    {
        private const string ConnectionString =
                "server=direct.dyna-studios.com;"
                + "uid=slaysher;"
                + "pwd=start123;"
                + "database=slaysher;"
                + "port=3306";

        public void Init(Server server)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();
            DAO dao = new DAO(connection);
            server.DAO = dao;
        }
    }
}
