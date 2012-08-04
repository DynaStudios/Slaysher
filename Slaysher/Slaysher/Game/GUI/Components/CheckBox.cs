using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Slaysher.Game.GUI;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components {
    public class CheckBox : Button
    {
        public override event EventHandler<EventArgs> Clicked;
        public virtual event EventHandler<EventArgs> Activated;
        public virtual event EventHandler<EventArgs> Deactivated;

        public Texture2D ActiveTexture { get; set; }
        public bool Active { get; set; }

        public CheckBox() : base("")
        {
            FillColor = Color.LightGreen;
        }

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
                drawingTextue = _buttonTexture ?? screenManager.BlankTexture;
            }

            Rectangle rectangle = new Rectangle((int)Position.X, (int)Position.Y, 20, 20);

            screenManager.SpriteBatch.Draw(drawingTextue, rectangle, FillColor);
        }

        protected override void OnClicked()
        {
            Active = !Active;
            if (Clicked == null)
            {
                Clicked(this, EventArgs.Empty);
            }
            if (Active)
            {
                if (Activated != null)
                {
                    Activated(this, EventArgs.Empty);
                }
            }
            else
            {
                if (Deactivated != null)
                {
                    Deactivated(this, EventArgs.Empty);
                }
            }
        }
    }
}
