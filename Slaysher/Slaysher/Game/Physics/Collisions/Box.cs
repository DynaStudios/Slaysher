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

        public static bool Intersect(Box box1, Box box2)
        {
            return (Math.Abs(box1.Location.X - box2.Location.X) * 2 < (box1.Width + box2.Width)) &&
         (Math.Abs(box1.Location.Y - box2.Location.Y) * 2 < (box1.Height + box2.Height));
        }
    }
}