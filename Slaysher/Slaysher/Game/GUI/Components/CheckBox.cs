using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components {
    public class CheckBox : Button
    {
        public Texture2D ActiveTexture { get; set; }
        public bool Active { get; set; }

        public CheckBox() : base("")
        { }

        public override void Draw(GameScreen gameScreen, GameTime gameTime)
        {
            ScreenManager screenManager = gameScreen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Texture2D drawingTextue;

            if (Active)
            {
                drawingTextue = ActiveTexture ??
                    screenManager.Game.Content.Load<Texture2D>("Images/Game/GUI/CheckBoxX");
            }
            else
            {
                drawingTextue = _buttonTexture ??
                    new Texture2D(screenManager.Game.GraphicsDevice, 20, 20, false, SurfaceFormat.Color);
            }

            Rectangle rectangle = new Rectangle((int)Position.X, (int)Position.Y, drawingTextue.Width, drawingTextue.Height);

            screenManager.SpriteBatch.Draw(drawingTextue, rectangle, FillColor);
        }
    }
}
