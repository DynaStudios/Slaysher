using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SlaysherNetworking.Game.Entities;

namespace Slaysher.Game.Entities
{
    public class ClientPlayer : Player
    {
        public void Update(Matrix chasedObjectsWorld)
        {
            HandleInput();
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Position.Y -= 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Position.Y += 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Position.X -= 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Position.X += 1.0f;
            }
        }
    }
}
