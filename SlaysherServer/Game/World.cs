using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlaysherServer;

namespace SlaysherServer.Game
{
    public class World
    {
        public Server Server { get; set; }

        public World(Server server)
        {
            Server = server;
        }

        public void Start()
        {
            //TODO: Check if a world allready exists

            //ELSE
            //Create new World;
        }

        /// <summary>
        /// Gets called 20 times every second!
        /// </summary>
        public void Tick()
        {
            //Do cool Tick Stuff here
        }
    }
}