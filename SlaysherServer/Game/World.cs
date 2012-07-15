using System;
using System.Collections.Generic;

using SlaysherServer;

namespace SlaysherServer.Game
{
    public class World
    {
        public Server Server { get; set; }

        public List<Pattern> Patterns { get; private set; }

        public List<Entity> Entities { get; private set; }

        public World(Server server)
        {
            Server = server;
            Patterns = new PatternGenerator(server.DAO).GetPatterns();
            Entities = new List<Entity>();
        }

        public void Start()
        {
            //TODO: Check if a world already exists

            //ELSE
            //Create new World;
        }

        /// <summary>
        /// Gets called 20 times every second!
        /// </summary>
        public void Tick(long serverTick)
        {
            //Do cool Tick Stuff here

            //Pattern Garbage Collection every 10 Seconds
            if (serverTick % 200 == 0)
            {
                patternGC();
            }
        }

        /// <summary>
        /// Pattern Garbage Collections
        ///
        /// Retrieves Player Positions und cleans unnessecary Patterns from memory.
        /// </summary>
        private void patternGC()
        {
            //TODO: Implement. Change List<Pattern> with a Dictionary<Vector2, Pattern>
        }
    }
}