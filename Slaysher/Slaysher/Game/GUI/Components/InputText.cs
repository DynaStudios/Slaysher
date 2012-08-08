using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Screens;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

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
        private readonly Dictionary<string, int> _charWidth;

        //Vars for Text Selection
        private bool _selectionInProgress;
        private int _selectionStart;
        private int _selectionEnd;

        private int _visibleSelectionStart;
        private int _visibleSelectionEnd;
        private bool _pastedSecurity;

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

            if (Text != string.Empty)
            {
                //Calculate Textposition
                Vector2 textSize = _font.MeasureString(Text);
                Vector2 textPosition = new Vector2(rec.Left + PaddingLeft,
                                                   rec.Center.Y - textSize.Y/2 + BorderThickness/2);

                spriteBatch.DrawString(_font, Text, textPosition, TextColor);
                if (_hasFocus && gameTime.TotalGameTime.Seconds%2 == 0)
                {
                    //Draw Cursor
                    var cursorX = Position.X + PaddingLeft + CalculateXPositionFromCursor(_cursorPosition);
                    Rectangle cursorRec = new Rectangle((int) cursorX, (int) Position.Y + 13, 2, (int) Size.Y - 20);
                    spriteBatch.Draw(borderTexture, cursorRec, Color.Red);
                }
            }

            //Draw Selection Rectangle
            if (_selectionInProgress || (_selectionStart != 0 || _selectionEnd != 0))
            {
                CheckedSelectionWrite();
                var selectionEnd = _visibleSelectionEnd;
                var selectionStart = _visibleSelectionStart;

                Rectangle selectionRec =
                    new Rectangle((int)(Position.X + PaddingLeft + CalculateXPositionFromCursor(selectionStart)),
                                  (int)Position.Y + 13, CalculateXPositionFromCursor(Math.Abs(selectionStart - selectionEnd)),
                                  (int) Size.Y - 20);
                spriteBatch.Draw(borderTexture, selectionRec, Color.Black * 0.3f);
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
                    if (FocusChanged != null)
                    {
                        FocusChanged(this, EventArgs.Empty);
                    }
                }
                else if (input.MouseState.LeftButton == ButtonState.Pressed)
                {
                    _cursorPosition = CalculateCursorPositionFromX((int) (Position.X + PaddingLeft), input.MouseState.X);
                    if (!_selectionInProgress)
                    {
                        _selectionInProgress = true;
                        _selectionEnd = 0;
                        _selectionStart = _cursorPosition;
                    }
                    else
                    {
                        _selectionEnd = _cursorPosition;
                    }
                }
                else if (input.MouseState.LeftButton == ButtonState.Released && _selectionInProgress)
                {
                    _selectionEnd = CalculateCursorPositionFromX((int)(Position.X + PaddingLeft), input.MouseState.X);
                    _selectionInProgress = false;
                }
            }
            else
            {
                if (input.LeftMouseClicked && _hasFocus)
                {
                    _hasFocus = false;
                    if (FocusChanged != null)
                    {
                        FocusChanged(this, EventArgs.Empty);
                    }
                }
            }

            if (_hasFocus)
            {
                if(input.KeyboardState.IsKeyDown(Keys.LeftControl) && input.KeyboardState.IsKeyDown(Keys.C))
                {
                    Clipboard.SetText(SelectedText());
                }
                if (input.KeyboardState.IsKeyDown(Keys.LeftControl) && input.KeyboardState.IsKeyDown(Keys.V))
                {
                    var insert = Clipboard.GetText();
                    if(insert != string.Empty && !_pastedSecurity)
                    {
                        if(insert.Length + Text.Length <= MaxChars) {
                            Text = Text.Insert(_cursorPosition, insert);
                        }
                        else
                        {
                            //Cut Insertion or forbid
                            var available = MaxChars - Text.Length;
                            Text = Text.Insert(_cursorPosition, insert.Substring(0, available));
                        }
                    }
                    _pastedSecurity = true;
                }
                else
                {
                    _pastedSecurity = false;
                }

                //Handle Keystrokes
                if (MaxChars == 0 || Text.Length + input.PressedKeys.Count <= MaxChars)
                {
                    HandleKeyboardInput(input.PressedKeys);
                }
                else
                {
                    var lenght = Text.Length;
                    if (lenght == MaxChars)
                    {
                        if (input.PressedKeys.Contains(Keys.Back))
                        {
                            HandleKeyboardInput(input.PressedKeys);
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

            if (cursorDelta != 0)
            {
                if (ValueChange != null)
                {
                    ValueChange(this, EventArgs.Empty);
                }
            }

            if (_cursorPosition == -1)
            {
                _cursorPosition = Text.Length;
            }
            else
            {
                if (pressedKeys.Contains(Keys.Delete))
                {
                    if(_selectionStart != 0 || _selectionEnd != 0)
                    {
                        SelectedRemove();
                        _cursorPosition = _selectionStart;
                        _selectionStart = 0;
                        _selectionEnd = 0;
                        CheckedSelectionWrite();
                    }
                    cursorDelta++;
                }
                else if (pressedKeys.Contains(Keys.Left))
                {
                    cursorDelta--;
                }
                else if (pressedKeys.Contains(Keys.Right))
                {
                    cursorDelta++;
                }
                else if (pressedKeys.Contains(Keys.Enter))
                {
                    if (EnterKey != null)
                    {
                        EnterKey(this, EventArgs.Empty);
                    }
                }
                _cursorPosition += cursorDelta;
                if (_cursorPosition > Text.Length)
                {
                    _cursorPosition = Text.Length;
                }
                else if (_cursorPosition == -1)
                {
                    _cursorPosition = 0;
                }
            }
            CalculateCharWidth();
        }

        private void CheckedSelectionWrite()
        {

            if (_selectionStart > _selectionEnd)
            {
                _visibleSelectionStart = _selectionEnd;
                _visibleSelectionEnd = _selectionStart;
            }
            else
            {
                _visibleSelectionStart = _selectionStart;
                _visibleSelectionEnd = _selectionEnd;
            }

        }

        private string SelectedText()
        {
            CheckedSelectionWrite();
            return Text.Substring(_visibleSelectionStart, _visibleSelectionEnd - 1);
        }

        private void SelectedRemove()
        {
            CheckedSelectionWrite();
            var amount = _visibleSelectionEnd - _visibleSelectionStart - 1;
            Text = Text.Remove(_visibleSelectionStart, amount);
        }

        private void CalculateCharWidth()
        {
            foreach (char c in Text)
            {
                string capital = c.ToString();
                if (!_charWidth.ContainsKey(capital))
                {
                    _charWidth.Add(capital, (int) _font.MeasureString(capital).X);
                }
            }
        }

        private int CalculateXPositionFromCursor(int cursorPosition)
        {
            if (cursorPosition > Text.Length)
            {
                cursorPosition = Text.Length - 1;
            }

            int xLength = 0;

            if(cursorPosition != -1) {
                string substring = Text.Substring(0, cursorPosition);
                foreach (char c in substring)
                {
                    string myChar = c.ToString();
                    xLength += _charWidth[myChar];
                }
            }

            return xLength;
        }

        private int CalculateCursorPositionFromX(int zeroXPosition, int xPosition)
        {
            int deltaX = zeroXPosition - xPosition;
            if(deltaX > 0)
            {
                return 0;
            }
            deltaX = Math.Abs(deltaX);
            int dummyCursorPosition = 0;
            bool searchingCursorPosition = true;
            while (searchingCursorPosition)
            {
                var calculation = CalculateXPositionFromCursor(dummyCursorPosition);

                if (calculation >= deltaX)
                {
                    return dummyCursorPosition;
                }
                if (dummyCursorPosition > Text.Length)
                {
                    searchingCursorPosition = false;
                }

                dummyCursorPosition++;
            }
            return Text.Length;
        }
    }
}