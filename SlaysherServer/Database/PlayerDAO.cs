using System.Data.Common;

using SlaysherNetworking.Game.World;

using SlaysherServer.Game.Models;
using SlaysherServer.Game.Entities;

namespace SlaysherServer.Database
{
    public class PlayerDAO
    {
        private DAO dao;
        private DbConnection Db { get { return dao.DBConnection; } }
        private DbCommand GetForClientCommand { get; set; }

        public PlayerDAO(DAO dao)
        {
            this.dao = dao;
            GetForClientCommand = Db.CreateCommand();
            GetForClientCommand.CommandText =
                    "SELECT id, nickname, hp, model, modelScaling, posX, posY, speed, texture"
                    + " FROM Player"
                    + " WHERE client=@client";
        }

        public ServerPlayer getForClient(Client client)
        {
            return getForClient(client.ClientId);
        }

        private WorldPosition readPosition(DbDataReader reader)
        {
            return new WorldPosition((float)reader["posX"], (float)reader["posY"]);
        }

        public ServerPlayer getForClient(int clientId)
        {
            lock (GetForClientCommand)
            {
                GetForClientCommand.Parameters.Clear();
                GetForClientCommand.SetParameter("client", clientId);

                using (DbDataReader reader = GetForClientCommand.ExecuteReader())
                {
                    return new ServerPlayer
                    {
                        DbId        = (int)reader["id"],
                        Id          = clientId,
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
            if (GetForClientCommand != null)
            {
                GetForClientCommand.Dispose();
                GetForClientCommand = null;
            }
        }
    }
}
