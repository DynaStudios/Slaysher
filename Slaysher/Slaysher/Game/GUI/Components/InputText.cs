using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public Color FocusColor { get; set; }
        public int BorderThickness { get; set; }

        public event EventHandler ValueChange;
        public event EventHandler FocusChanged;
        public event EventHandler EnterKey;

        //Internal Vars
        protected float PaddingLeft { get; set; }
        private bool _hasFocus;
        private SpriteFont _font;
        private int _cursorPosition;

        public InputText()
        {
            Size = new Vector2(190, 40);
            FillColor = Color.Gray;
            TextColor = Color.White;

            BorderThickness = 4;
            BorderColor = Color.White;
            FocusColor = Color.Yellow;
            PaddingLeft = 10;
            MaxChars = 15;

            Text = "Test";
            _cursorPosition = Text.Length;
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

            if (Text != string.Empty) { 
                //Calculate Textposition
                _font = _font ?? font;
                Vector2 textSize = _font.MeasureString(Text);
                Vector2 textPosition = new Vector2(rec.Left + PaddingLeft, rec.Center.Y - textSize.Y / 2 + BorderThickness / 2);

                var textWithCursor = Text;
                if(_hasFocus) {
                    if(_cursorPosition == Text.Length)
                    {
                        textWithCursor += '|';
                    }
                    else
                    {
                        textWithCursor = textWithCursor.Insert(_cursorPosition, "|");
                    }
                }

                spriteBatch.DrawString(_font, textWithCursor, textPosition, TextColor);
            }

            //Draw Border
            var borderColor = (!_hasFocus) ? BorderColor : FocusColor;
            var borderTexture = BorderTexture ?? gameScreen.ScreenManager.BlankTexture;
            spriteBatch.Draw(borderTexture, new Rectangle(rec.Left, rec.Top, rec.Width, BorderThickness),
                             borderColor);
            spriteBatch.Draw(borderTexture, new Rectangle(rec.Left, rec.Top, BorderThickness, rec.Height),
                             borderColor);
            spriteBatch.Draw(borderTexture,
                             new Rectangle(rec.Right - BorderThickness, rec.Top, BorderThickness, rec.Height),
                             borderColor);
            spriteBatch.Draw(borderTexture,
                             new Rectangle(rec.Left, rec.Bottom - BorderThickness, rec.Width, BorderThickness),
                             borderColor);
        }


        public void HandleInput(InputState input)
        {
            if (input.MouseState.IsMouseIn(Position, Size))
            {
                if (input.LeftMouseClicked && !_hasFocus)
                {
                    _hasFocus = true;
                }
                else if(input.LeftMouseClicked) {
                    _cursorPosition = CalculateCursorPosition(input.MouseState, Text, _font.MeasureString(Text).X);
                }
            }
            else
            {
                if (input.LeftMouseClicked && _hasFocus)
                {
                    _hasFocus = false;
                }
            }

            if (_hasFocus)
            {
                //Handle Keystrokes
                if(MaxChars == 0 || Text.Length + input.PressedKeys.Count <= MaxChars)
                {
                    HandleKeyboardInput(input.PressedKeys);
                }
                else
                {
                    var lenght = Text.Length;
                    if(lenght == MaxChars) {
                        if(input.PressedKeys.Contains(Keys.Back))
                        {
                            HandleKeyboardInput(input.PressedKeys);
                            return;
                        }    
                    }
                    else 
                    {
                        HandleKeyboardInput(input.PressedKeys.GetRange(0, 1));
                    }
                }
            }
        }

        private void HandleKeyboardInput(List<Keys> pressedKeys)
        {
            var cursorTemp = Text.Length;
            Text = Extensions.HandleKeyboardInput(Text, pressedKeys, _cursorPosition);
            var cursorDelta = Text.Length - cursorTemp;

            if (_cursorPosition == -1) 
            {
                _cursorPosition = Text.Length;
            }
            else
            {
                if(pressedKeys.Contains(Keys.Delete))
                {
                    cursorDelta++;
                }
                else if(pressedKeys.Contains(Keys.Left))
                {
                    cursorDelta--;
                }
                else if(pressedKeys.Contains(Keys.Right))
                {
                    cursorDelta++;
                }
                _cursorPosition += cursorDelta;
                if(_cursorPosition > Text.Length)
                {
                    _cursorPosition = Text.Length;
                }
                else if(_cursorPosition == -1)
                {
                    _cursorPosition = 0;
                }
            }
        }

        private int CalculateCursorPosition(MouseState mouseState, string text, float measuredWidth)
        {
            var xPos = 0;
            if(text != string.Empty) {
                xPos = (int)Math.Floor(Math.Abs(Position.X - mouseState.X - PaddingLeft) / (measuredWidth / text.Length));
            }

            return xPos - 1;
        }
    }
}