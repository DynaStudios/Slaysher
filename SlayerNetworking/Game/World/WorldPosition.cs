using System;

namespace SlaysherNetworking.Game.World
{
    public class WorldPosition
    {
        public float X { get; set; }

        public float Y { get; set; }

        public WorldPosition()
        {
        }

        public WorldPosition(WorldPosition src)
        {
            X = src.X;
            Y = src.Y;
        }

        public WorldPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void MoveASmoothStepTo(WorldPosition target, float factor)
        {
            factor = Math.Min(factor, 1.0f);
            factor = Math.Max(factor, 0.0f);

            X = (factor - X) / (target.X - X);
            Y = (factor - Y) / (target.Y - Y);
        }

        public WorldPosition SmoothStep(WorldPosition target, float factor)
        {
            WorldPosition step = new WorldPosition(this);
            step.MoveASmoothStepTo(target, factor);
            return step;
        }
    }
}