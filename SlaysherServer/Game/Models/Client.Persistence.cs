using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Game.World;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public void Save()
        {
            //TODO: Implement
        }

        public Player Load()
        {
            //TODO: DAO is missing atm
            Player newInstance = new Player
                {Health = 100, Nickname = "TestUser" + ClientId, Position = new WorldPosition(0, 0)};

            //Debug Player Info until Database implemented

            return newInstance;
        }
    }
}