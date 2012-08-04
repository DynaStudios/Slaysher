using Microsoft.Xna.Framework;

using SlaysherNetworking.Game.World;

namespace Slaysher.Graphics
{
    public static class Extensions
    {
        public static Vector3 CreateOnSurfacePosition(this WorldPosition worldPosition)
        {
            return new Vector3(worldPosition.X, 0, worldPosition.Y);
        }
    }
}
