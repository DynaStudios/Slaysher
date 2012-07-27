using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public class Button : GuiItem
    {
        public Vector2 Position { get; set; }

        //Button Properties
        public Vector2 Size { get; set; }

        public Color FillColor { get; set; }
        public Color HoverColor { get; set; }
        public Color BorderColor { get; set; }
        public Color TextColor { get; set; }
        
        public float Alpha { get; set; }
        public int BorderThickness { get; set; }

        public string Text { get; protected set; }

        public event EventHandler<EventArgs> Clicked;

        private bool _isHovered;
        private Texture2D _buttonTexture;

        public Button(string buttonText)
        {
            Text = buttonText;

            InitVars();
        }

        public Button(string buttonText, Texture2D backgroundTexture)
        {
            Text = buttonText;
            _buttonTexture = backgroundTexture;

            InitVars();
        }

        public void InitVars()
        {
            //Set Button Default Values
            Size = new Vector2(250, 75);
            FillColor = Color.Orange;
            HoverColor = Color.Red;
            BorderColor = Color.Black;
            TextColor = Color.White;
            Alpha = 1f;
            BorderThickness = 4;
        }

        public float GetWidth(GameScreen gameScreen)
        {
            return (int) Size.X;
        }

        public float GetHeight(GameScreen gameScreen)
        {
            return Size.Y;
        }

        public void Update(GameScreen gameScreen, GameTime gameTime)
        {
            //todo: I don't know yet ^^
        }

        public void Draw(GameScreen gameScreen, GameTime gameTime)
        {

            SpriteBatch spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            SpriteFont font = gameScreen.ScreenManager.Font;

            if (_buttonTexture == null)
            {
                _buttonTexture = gameScreen.ScreenManager.BlankTexture;
            }

            Rectangle rec = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

            //Fill Button
            if (_isHovered)
            {
                spriteBatch.Draw(_buttonTexture, rec, HoverColor * Alpha);
            }
            else 
            { 
                spriteBatch.Draw(_buttonTexture, rec, FillColor * Alpha);
            }

            //Draw Border
            spriteBatch.Draw(_buttonTexture, new Rectangle(rec.Left, rec.Top, rec.Width, BorderThickness), BorderColor * Alpha);
            spriteBatch.Draw(_buttonTexture, new Rectangle(rec.Left, rec.Top, BorderThickness, rec.Height), BorderColor * Alpha);
            spriteBatch.Draw(_buttonTexture, new Rectangle(rec.Right - BorderThickness, rec.Top, BorderThickness, rec.Height), BorderColor * Alpha);
            spriteBatch.Draw(_buttonTexture, new Rectangle(rec.Left, rec.Bottom - BorderThickness, rec.Width, BorderThickness), BorderColor * Alpha);

            //Draw the Text centered in the button
            Vector2 textSize = font.MeasureString(Text);
            Vector2 textPosition = new Vector2(rec.Center.X, rec.Center.Y) - textSize / 2f;
            textPosition.X = (int) textPosition.X;
            textPosition.Y = (int) textPosition.Y;
            spriteBatch.DrawString(font, Text, textPosition, TextColor * Alpha);

        }

        protected virtual void OnClicked()
        {
            if (Clicked != null)
            {
                Clicked(this, EventArgs.Empty);
            }
        }

        public void HandleInput(InputState input)
        {

            var mousePosition = new Vector2(input.MouseState.X, input.MouseState.Y);

            var posX = Position.X;
            var posY = Position.Y;
            var sizeX = Size.X;
            var sizeY = Size.Y;

            if (mousePosition.X >= Position.X && mousePosition.Y >= Position.Y && mousePosition.X <= Position.X + Size.X && mousePosition.Y <= Position.Y + Size.Y)
            {
                _isHovered = true;
                if (input.MouseState.LeftButton == ButtonState.Pressed)
                {
                    OnClicked();
                }
            }
            else
            {
                _isHovered = false;
            }

        }
    }
}