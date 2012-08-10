using System;

using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Game.Entities
{
    public static class EntityExtensions
    {

        public static void PrepareToMove(this IEntity entity, float direction, float speed)
        {
            entity.PreparedDirection = direction;
            entity.PreparedSpeed = speed;
        }

        public static bool ExecutePreparedMove(this IEntity entity, TimeSpan totalTime)
        {
            if (entity.PreparedDirection == null || entity.PreparedSpeed == null)
            {
                return false;
            }

            entity.StopMoving(totalTime);
            if (entity.PreparedSpeed > 0)
            {
                entity.SetSpeed((float)entity.PreparedSpeed);
                entity.Move(totalTime, (float)entity.PreparedDirection);
            }
            entity.PreparedDirection = null;
            entity.PreparedSpeed = null;
            return true;
        }

        public static void Move(this IEntity entity, TimeSpan totalTime, float direction)
        {
            entity.ExecuteMovement(totalTime);
            // using now position for the next movement
            entity.StartPosition = new WorldPosition(entity.Position);
            entity.MovementStarted = totalTime;
            entity.Turn(direction);
        }

        public static void Turn(this IEntity entity, float direction)
        {
            // XXX: smothered turnning would be nice
            entity.Direction = direction;
        }

        public static void StopMoving(this IEntity entity, TimeSpan? totalTime)
        {

            // direction should stay as it is
            if (totalTime != null)
            {
                entity.ExecuteMovement((TimeSpan)totalTime);
            }
            entity.StartPosition = null;
            entity.MovementStarted = null;
        }

        public static void ExecuteMovement(this IEntity entity, TimeSpan current)
        {
            if (entity.MovementStarted == null || entity.StartPosition == null)
            {
                return;
            }
            TimeSpan movedTime = current - (TimeSpan)entity.MovementStarted;
            float distance = (float)(movedTime.TotalMilliseconds * entity.SpeedMeterPerMillisecond);

            entity.Position.X = entity.StartPosition.X + (float)(Math.Sin(entity.Direction * Math.PI / 180) * distance);
            entity.Position.Y = entity.StartPosition.Y - (float)(Math.Cos(entity.Direction * Math.PI / 180) * distance);
        }

        /// <summary>
        /// Speed in meter per second
        /// </summary>
        public static float GetSpeed(this IEntity entity)
        {
            return entity.SpeedMeterPerMillisecond * 1000;
        }

        public static void SetSpeed(this IEntity entity, float value)
        {
            entity.SpeedMeterPerMillisecond = value / 1000;
        }

        public static bool IsMoving(this IEntity entity)
        {
            return entity.MovementStarted != null;
        }
    }

    public interface IEntity
    {
        int Id { get; set; }

        float ModelScaling { get; set; }

        int ModelId { get; set; }

        int TextureId { get; set; }

        int Health { get; set; }

        float SpeedMeterPerMillisecond { get; set; }

        WorldPosition Position { get; set; }

        // position where a movement has started
        // should be null at init
        WorldPosition StartPosition { get; set;}

        // should be null at init
        TimeSpan? MovementStarted { get; set; }

        float Direction { get; set; }

        float? PreparedDirection { get; set; }
        float? PreparedSpeed { get; set; }

        void Tick(TimeSpan totalRunTime);
    }
}
