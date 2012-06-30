using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Slaysher.Game.Physics.Collisions
{
    public class AABB
    {
        public Vector3 frontBox;
        public Vector3 backBox;

        public AABB(Vector3 front, Vector3 back)
        {
            frontBox = front;
            backBox = back;
        }

        public static bool Collides(AABB box1, AABB box2)
        {
            if (box1.frontBox.X > box2.backBox.X) return false;
            if (box1.frontBox.Y > box2.backBox.Y) return false;
            if (box1.frontBox.Z > box2.backBox.Z) return false;
            if (box1.backBox.X < box2.frontBox.X) return false;
            if (box1.backBox.Y < box2.frontBox.Y) return false;
            if (box1.backBox.Z < box2.frontBox.Z) return false;

            return true;
        }

        public static bool Collides2D(AABB box1, AABB box2)
        {
            if (box1.frontBox.X > box2.backBox.X) return false;
            if (box1.frontBox.Y > box2.backBox.Y) return false;
            if (box1.backBox.X < box2.frontBox.X) return false;
            if (box1.backBox.Y < box2.frontBox.Y) return false;

            return true;
        }
    }
}