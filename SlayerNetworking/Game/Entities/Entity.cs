using System;

using SlaysherNetworking.Game.World;

namespace SlaysherNetworking.Game.Entities
{
    public static class EntetyExtensions
    {

        public static void PrepareToMove(this IEntity entity, float direction, float speed)
        {
            entity._preparedDirection = direction;
            entity._preparedSpeed = speed;
        }



        public static bool ExecutePreparedMove(this IEntity entity, TimeSpan totalTime)
        {
            if (entity._preparedDirection == null || entity._preparedSpeed == null)
            {
                return false;
            }

            entity.StopMoving(totalTime);
            if (entity._preparedSpeed > 0)
            {
                entity.SetSeed((float)entity._preparedSpeed);
                entity.Move(totalTime, (float)entity._preparedDirection);
            }
            entity._preparedDirection = null;
            entity._preparedSpeed = null;
            return true;
        }

        public static void Move(this IEntity entity, TimeSpan totalTime, float direction)
        {
            entity.ExecuteMovement(totalTime);
            // using now position for the next movement
            entity._startPosition = new WorldPosition(entity.Position);
            entity._movemetStarted = totalTime;
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
            entity._startPosition = null;
            entity._movemetStarted = null;
        }

        public static void ExecuteMovement(this IEntity entity, TimeSpan current)
        {
            if (entity._movemetStarted == null || entity._startPosition == null)
            {
                return;
            }
            TimeSpan movedTime = current - (TimeSpan)entity._movemetStarted;
            float distance = (float)(movedTime.TotalMilliseconds * entity.SpeedMeeterPerMillisecond);

            entity.Position.X = entity._startPosition.X + (float)(Math.Sin(entity.Direction * Math.PI / 180) * distance);
            entity.Position.Y = entity._startPosition.Y - (float)(Math.Cos(entity.Direction * Math.PI / 180) * distance);
        }

        /// <summary>
        /// Speed in meter per second
        /// </summary>
        public static float GetSpeed(this IEntity entity)
        {
            return entity.SpeedMeeterPerMillisecond * 1000;
        }

        public static void SetSeed(this IEntity entity, float value)
        {
            entity.SpeedMeeterPerMillisecond = value / 1000;
        }

        public static bool IsMoving(this IEntity entity)
        {
            return entity._movemetStarted != null;
        }
    }

    public interface IEntity
    {
        int Id { get; set; }

        float ModelScaling { get; set; }

        int ModelId { get; set; }

        int TextureId { get; set; }

        int Health { get; set; }

        float SpeedMeeterPerMillisecond { get; set; }

        WorldPosition Position { get; set; }

        // position where a movement has started
        // should be null at init
        WorldPosition _startPosition { get; set;}

        // should be null at init
        TimeSpan? _movemetStarted { get; set; }

        float Direction { get; set; }

        float? _preparedDirection { get; set; }
        float? _preparedSpeed { get; set; }

        void Tick(TimeSpan totalRunTime);
    }
}
