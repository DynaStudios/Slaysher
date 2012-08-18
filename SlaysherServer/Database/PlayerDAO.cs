using Npgsql;

using SlaysherNetworking.Game.World;

using SlaysherServer.Game.Models;
using SlaysherServer.Game.Entities;

namespace SlaysherServer.Database
{
    public class PlayerDAO
    {
        private DAO dao;
        private NpgsqlConnection Db { get { return dao.DBConnection; } }
        private NpgsqlCommand _getForClientCommand = null;
        private NpgsqlCommand GetForClientCommand
        {
            get
            {
                if (_getForClientCommand == null)
                {
                    _getForClientCommand = new NpgsqlCommand(
                            "SELECT id, nickname, hp, model, modelScaling, posX, posY, speed, texture"
                            + " FROM Player"
                            + " WHERE client=@client"
                            , Db);
                }
                return _getForClientCommand;
            }
        }

        public PlayerDAO(DAO dao)
        {
            this.dao = dao;
        }

        public ServerPlayer getForClient(Client client)
        {
            return getForClient(client.ClientId);
        }

        private WorldPosition readPosition(NpgsqlDataReader reader)
        {
            return new WorldPosition((float)reader["posX"], (float)reader["posY"]);
        }

        public ServerPlayer getForClient(int clientId)
        {
            lock (GetForClientCommand)
            {
                GetForClientCommand.Parameters.AddWithValue("client", clientId);
                using (NpgsqlDataReader reader = GetForClientCommand.ExecuteReader())
                {
                    return new ServerPlayer
                    {
                        DbId        = (int)reader["id"],
                        Nickname    = (string)reader["nickname"],
                        Health      = (int)reader["hp"],
                        ModelId     = (int)reader["model"],
                        ModelScaling= (float)reader["modelScaling"],
                        Position    = readPosition(reader),
                        Speed       = (float)reader["speed"],
                        TextureId   = (int)reader["texture"]
                    };
                }
            }
        }

        public void save(ServerPlayer player)
        {

        }

        public void Dispose()
        {
            if (_getForClientCommand != null)
            {
                _getForClientCommand.Dispose();
            }
        }
    }
}
