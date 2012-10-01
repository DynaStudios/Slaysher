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
            return Server.DAO.Player.getForClient(this);
        }
    }
}
