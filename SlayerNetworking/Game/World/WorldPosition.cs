using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlaysherNetworking.Game.World
{
    public class WorldPosition
    {
        public float X { get; set; }

        public float Y { get; set; }

        public WorldPosition() { }

        public WorldPosition(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}