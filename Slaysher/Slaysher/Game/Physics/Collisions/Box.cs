using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Slaysher.Game.Physics.Collisions
{
    public class Box
    {
        public Vector2 Location { get; set; }

        public int Height;
        public int Width;

        public Box(Vector2 location, Vector2 size)
        {
            Location = location;
            Height = (int)size.Y;
            Width = (int)size.X;
        }
    }
}