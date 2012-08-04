using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
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
            Size = new Vector2(25, 25);
        }

        public override void Draw(GameScreen gameScreen, GameTime gameTime)
        {
            base.Draw(gameScreen, gameTime);
            ScreenManager screenManager = gameScreen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            if (Active)
            {
                Texture2D drawingTextue = ActiveTexture ??
                                          screenManager.Game.Content.Load<Texture2D>("Images/GUI/CheckBoxX");

                Vector2 textureSize = Size;
                textureSize.X -= BorderThickness;
                textureSize.Y -= BorderThickness;

                Vector2 texturePosition = Position;
                texturePosition.X += BorderThickness;
                texturePosition.Y += BorderThickness;

                Rectangle rectangle = new Rectangle((int) texturePosition.X, (int) texturePosition.Y,
                                                    (int) textureSize.X - BorderThickness,
                                                    (int) textureSize.Y - BorderThickness);

                spriteBatch.Draw(drawingTextue, rectangle, FillColor);
            }
        }

        protected override void OnClicked()
        {
            Active = !Active;
            if (Clicked != null)
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