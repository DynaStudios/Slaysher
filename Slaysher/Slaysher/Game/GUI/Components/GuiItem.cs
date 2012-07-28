using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public interface IGuiItem
    {
        Vector2 Position { get; set; }

        float GetWidth(GameScreen gameScreen);
        float GetHeight(GameScreen gameScreen);
        void Update(GameScreen gameScreen, GameTime gameTime);
        void Draw(GameScreen gameScreen, GameTime gameTime);
        void HandleInput(InputState input);
    }
}