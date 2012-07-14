using System;
using System.Collections.Generic;

using SlaysherServer;

namespace SlaysherServer.Game
{
    public class World
    {
        public Server Server { get; set; }
        public List<Pattern> Patterns { get; private set; }

        public World(Server server)
        {
            Server = server;
            this.Patterns = new PatternGenerator(server.DAO).GetPatterns();

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