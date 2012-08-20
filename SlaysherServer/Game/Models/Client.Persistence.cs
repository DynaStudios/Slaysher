using SlaysherServer.Game.Entities;

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

        public ServerPlayer Load()
        {
            //TODO: DAO is missing atm
            ServerPlayer newInstance = new ServerPlayer
                {
                    Id = ClientId,
                    Health = 100,
                    Nickname = "TestUser" + ClientId,
                    Position = new WorldPosition(0, 0),
                    Speed = 10.0f
                };

            //Debug Player Info until Database implemented

            return newInstance;
        }
    }
}