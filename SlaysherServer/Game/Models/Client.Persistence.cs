using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Player newInstance = new Player();

            //Debug Player Info until Database implemented
            newInstance.Health = 100;
            newInstance.Nickname = "TestUser" + ClientId;
            newInstance.Position = new WorldPosition(0, 0);

            return newInstance;
        }
    }
}