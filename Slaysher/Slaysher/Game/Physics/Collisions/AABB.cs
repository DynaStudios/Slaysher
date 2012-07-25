using Microsoft.Xna.Framework;

namespace Slaysher.Game.Physics.Collisions
{
    public class AABB
    {
        public Vector3 FrontBox;
        public Vector3 BackBox;

        public AABB(Vector3 front, Vector3 back)
        {
            FrontBox = front;
            BackBox = back;
        }

        public static bool Collides(AABB box1, AABB box2)
        {
            if (box1.FrontBox.X > box2.BackBox.X) return false;
            if (box1.FrontBox.Y > box2.BackBox.Y) return false;
            if (box1.FrontBox.Z > box2.BackBox.Z) return false;
            if (box1.BackBox.X < box2.FrontBox.X) return false;
            if (box1.BackBox.Y < box2.FrontBox.Y) return false;
            if (box1.BackBox.Z < box2.FrontBox.Z) return false;

            return true;
        }

        public static bool Collides2D(AABB box1, AABB box2)
        {
            if (box1.FrontBox.X > box2.BackBox.X) return false;
            if (box1.FrontBox.Y > box2.BackBox.Y) return false;
            if (box1.BackBox.X < box2.FrontBox.X) return false;
            if (box1.BackBox.Y < box2.FrontBox.Y) return false;

            return true;
        }
    }
}