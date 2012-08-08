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
        private Dictionary<string, int> _charWidth; 

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

            Text = "";
            _charWidth = new Dictionary<string, int>();
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
            _font = _font ?? font;

            Rectangle rec = new Rectangle((int) Position.X, (int) Position.Y, (int) Size.X, (int) Size.Y);

            var bgTexture = BackgroundTexture ?? gameScreen.ScreenManager.BlankTexture;
            var borderColor = (!_hasFocus) ? BorderColor : FocusColor;
            var borderTexture = BorderTexture ?? gameScreen.ScreenManager.BlankTexture;

            //Draw Rectangle
            spriteBatch.Draw(bgTexture, rec, FillColor);

            if (Text != string.Empty) { 
                //Calculate Textposition
                Vector2 textSize = _font.MeasureString(Text);
                Vector2 textPosition = new Vector2(rec.Left + PaddingLeft, rec.Center.Y - textSize.Y / 2 + BorderThickness / 2);

                spriteBatch.DrawString(_font, Text, textPosition, TextColor);
                if (_hasFocus && gameTime.TotalGameTime.Seconds % 2 == 0)
                {
                    //Draw Cursor
                    var cursorX = Position.X + PaddingLeft + CalculateXPositionFromCursor(_cursorPosition);
                    Rectangle cursorRec = new Rectangle((int)cursorX, (int)Position.Y + 13, 2, (int)Size.Y - 20);
                    spriteBatch.Draw(borderTexture, cursorRec, Color.Red);
                }
            }

            //Draw Border
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
                else if(input.LeftMouseClicked)
                {
                    _cursorPosition = CalculateCursorPositionFromX((int) (Position.X + PaddingLeft), input.MouseState.X);
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
            CalculateCharWidth();
        }

        private void CalculateCharWidth()
        {
            foreach (char c in Text)
            {
                string capital = c.ToString();
                if(!_charWidth.ContainsKey(capital)) {
                    _charWidth.Add(capital, (int) _font.MeasureString(capital).X);
                }
            }
        }

        private int CalculateXPositionFromCursor(int cursorPosition)
        {
            if(cursorPosition > Text.Length)
            {
                cursorPosition = Text.Length - 1;
            }

            int xLength = 0;
            string substring = Text.Substring(0, cursorPosition);
            foreach (char c in substring)
            {
                string myChar = c.ToString();
                xLength += _charWidth[myChar];
            }

            return xLength;
        }

        private int CalculateCursorPositionFromX(int zeroXPosition, int xPosition)
        {
            int deltaX = Math.Abs(zeroXPosition - xPosition);
            int dummyCursorPosition = 1;
            bool searchingCursorPosition = true;
            while (searchingCursorPosition)
            {
                var calculation = CalculateXPositionFromCursor(dummyCursorPosition);
                
                if( calculation >= deltaX)
                {
                    return dummyCursorPosition;
                }
                else if(dummyCursorPosition > Text.Length)
                {
                    searchingCursorPosition = false;
                }

                dummyCursorPosition++;
            }
            return Text.Length;
        }
    }
}