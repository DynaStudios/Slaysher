using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public class Button : GuiItem
    {
        public Vector2 Position { get; set; }

        public string Text { get; protected set; }

        public Button(string buttonText)
        {
            Text = buttonText;
        }

        public float GetWidth(GameScreen gameScreen)
        {
            throw new System.NotImplementedException();
        }

        public float GetHeight(GameScreen gameScreen)
        {
            throw new System.NotImplementedException();
        }

        public void Update(GameScreen gameScreen, GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

        public void Draw(GameScreen gameScreen, GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

        public void HandleInput(InputState input)
        {
            throw new System.NotImplementedException();
        }
    }
}