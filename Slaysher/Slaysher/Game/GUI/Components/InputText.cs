using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public class InputText : IGuiItem
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public int ZIndex { get; set; }
        public SoundEffect HoverSound { get; set; }
        public SoundEffect ClickSound { get; set; }

        public string Text { get; set; }
        public int MaxChars { get; set; }

        public Color TextColor { get; set; }
        public Color FillColor { get; set; }

        public Texture2D BackgroundTexture { get; set; }

        public Texture2D BorderTexture { get; set; }
        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }

        public event EventHandler ValueChange;
        public event EventHandler FocusChanged;
        public event EventHandler EnterKey;

        //Internal Vars
        protected float PaddingLeft { get; set; }
        private bool _hasFocus;

        public InputText()
        {
            Size = new Vector2(190, 40);
            FillColor = Color.Gray;
            TextColor = Color.White;

            BorderThickness = 4;
            BorderColor = Color.White;
            PaddingLeft = 10;

            Text = "Test";
        }

        public float GetWidth(GameScreen gameScreen)
        {
            return Size.X;
        }

        public float GetHeight(GameScreen gameScreen)
        {
            return Size.Y;
        }

        public void Update(GameScreen gameScreen, GameTime gameTime)
        {
            
        }

        public void Draw(GameScreen gameScreen, GameTime gameTime)
        {
            SpriteBatch spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            SpriteFont font = gameScreen.ScreenManager.Font;

            Rectangle rec = new Rectangle((int) Position.X, (int) Position.Y, (int) Size.X, (int) Size.Y);

            var bgTexture = BackgroundTexture ?? gameScreen.ScreenManager.BlankTexture;

            //Draw Rectangle
            spriteBatch.Draw(bgTexture, rec, FillColor);

            if (Text != "") { 
                //Calculate Textposition
                Vector2 textSize = font.MeasureString(Text);
                Vector2 textPosition = new Vector2(rec.Left + PaddingLeft, rec.Center.Y - textSize.Y / 2 + BorderThickness / 2);

                spriteBatch.DrawString(font, Text, textPosition, TextColor);
            }

            //Draw Border
            var borderTexture = BorderTexture ?? gameScreen.ScreenManager.BlankTexture;
            spriteBatch.Draw(borderTexture, new Rectangle(rec.Left, rec.Top, rec.Width, BorderThickness),
                             BorderColor);
            spriteBatch.Draw(borderTexture, new Rectangle(rec.Left, rec.Top, BorderThickness, rec.Height),
                             BorderColor);
            spriteBatch.Draw(borderTexture,
                             new Rectangle(rec.Right - BorderThickness, rec.Top, BorderThickness, rec.Height),
                             BorderColor);
            spriteBatch.Draw(borderTexture,
                             new Rectangle(rec.Left, rec.Bottom - BorderThickness, rec.Width, BorderThickness),
                             BorderColor);
        }


        public void HandleInput(InputState input)
        {
            
        }
    }
}