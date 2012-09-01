using System;
using System.Data.Common;

namespace SlaysherServer.Database
{
    public class DAO : IDisposable
    {
        public DbConnection DBConnection { get; private set; }

        public GameObjectDAO GameObjectDAO { get; private set; }

        public PatternTypeDAO PatternTypeDAO { get; private set; }

        public PlayerDAO Player { get; private set; }

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