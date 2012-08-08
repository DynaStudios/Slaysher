using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.IO;

namespace Slaysher.Game.GUI
{
    public static class Extensions
    {
        public static bool IsMouseIn(this MouseState mouseState, Vector2 position, Vector2 size)
        {
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);

            return mousePosition.X >= position.X
                && mousePosition.Y >= position.Y
                && mousePosition.X <= position.X + size.X
                && mousePosition.Y <= position.Y + size.Y;
        }

        public static string HandleKeyboardInput(string text, List<Keys> keys, int cursorPosition = -1)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            if (keys.Count != 0) { 
                KeyboardState keyboard = Keyboard.GetState();
                cursorPosition = (cursorPosition != -1) ? cursorPosition - 1 : stringBuilder.Length - 1;
                if (cursorPosition >= stringBuilder.Length)
                    cursorPosition = stringBuilder.Length - 1;

                foreach (Keys key in keys)
                {
                    if (key == Keys.LeftShift || key == Keys.Enter || key == Keys.CapsLock || key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down || key == Keys.LeftControl)
                    {
                        continue;
                    }
                    if (key == Keys.Back)
                    {
                        if (stringBuilder.Length != 0) { 
                            stringBuilder.Remove(cursorPosition, 1);
                            cursorPosition++;
                        }
                        continue;
                    }
                    if (key == Keys.Delete)
                    {
                        if (stringBuilder.Length != 0 && cursorPosition != stringBuilder.Length - 1)
                        {
                            stringBuilder.Remove(cursorPosition + 1, 1);
                            cursorPosition++;
                        }
                        continue;
                    }

                    if (keyboard.IsKeyDown(Keys.LeftControl))
                    {
                        continue;
                    }
                    bool shiftPressed = keyboard.IsKeyDown(Keys.LeftShift);
                    bool capsLock = keyboard.IsKeyDown(Keys.CapsLock);
                    bool numLock = keyboard.IsKeyDown(Keys.NumLock);

                    var inputText = TranslateChar(key, shiftPressed, capsLock, numLock);
                    if (inputText != (char)0) { 
                        stringBuilder.Insert(cursorPosition + 1, inputText);
                        cursorPosition++;
                    }

                }
            }
            return stringBuilder.ToString();
        }

        public static char TranslateChar(Keys key, bool shift, bool capsLock, bool numLock)
        {
            switch (key)
            {
                case Keys.A: return TranslateAlphabetic('a', shift, capsLock);
                case Keys.B: return TranslateAlphabetic('b', shift, capsLock);
                case Keys.C: return TranslateAlphabetic('c', shift, capsLock);
                case Keys.D: return TranslateAlphabetic('d', shift, capsLock);
                case Keys.E: return TranslateAlphabetic('e', shift, capsLock);
                case Keys.F: return TranslateAlphabetic('f', shift, capsLock);
                case Keys.G: return TranslateAlphabetic('g', shift, capsLock);
                case Keys.H: return TranslateAlphabetic('h', shift, capsLock);
                case Keys.I: return TranslateAlphabetic('i', shift, capsLock);
                case Keys.J: return TranslateAlphabetic('j', shift, capsLock);
                case Keys.K: return TranslateAlphabetic('k', shift, capsLock);
                case Keys.L: return TranslateAlphabetic('l', shift, capsLock);
                case Keys.M: return TranslateAlphabetic('m', shift, capsLock);
                case Keys.N: return TranslateAlphabetic('n', shift, capsLock);
                case Keys.O: return TranslateAlphabetic('o', shift, capsLock);
                case Keys.P: return TranslateAlphabetic('p', shift, capsLock);
                case Keys.Q: return TranslateAlphabetic('q', shift, capsLock);
                case Keys.R: return TranslateAlphabetic('r', shift, capsLock);
                case Keys.S: return TranslateAlphabetic('s', shift, capsLock);
                case Keys.T: return TranslateAlphabetic('t', shift, capsLock);
                case Keys.U: return TranslateAlphabetic('u', shift, capsLock);
                case Keys.V: return TranslateAlphabetic('v', shift, capsLock);
                case Keys.W: return TranslateAlphabetic('w', shift, capsLock);
                case Keys.X: return TranslateAlphabetic('x', shift, capsLock);
                case Keys.Y: return TranslateAlphabetic('y', shift, capsLock);
                case Keys.Z: return TranslateAlphabetic('z', shift, capsLock);

                case Keys.D0: return (shift) ? '=' : '0';
                case Keys.D1: return (shift) ? '!' : '1';
                case Keys.D2: return (shift) ? '"' : '2';
                case Keys.D3: return (shift) ? '§' : '3';
                case Keys.D4: return (shift) ? '$' : '4';
                case Keys.D5: return (shift) ? '%' : '5';
                case Keys.D6: return (shift) ? '&' : '6';
                case Keys.D7: return (shift) ? '/' : '7';
                case Keys.D8: return (shift) ? '(' : '8';
                case Keys.D9: return (shift) ? ')' : '9';

                case Keys.Add: return '+';
                case Keys.Divide: return '/';
                case Keys.Multiply: return '*';
                case Keys.Subtract: return '-';

                case Keys.Space: return ' ';
                case Keys.Tab: return '\t';

                case Keys.Decimal: if (numLock && !shift) return '.'; break;
                case Keys.NumPad0: if (numLock && !shift) return '0'; break;
                case Keys.NumPad1: if (numLock && !shift) return '1'; break;
                case Keys.NumPad2: if (numLock && !shift) return '2'; break;
                case Keys.NumPad3: if (numLock && !shift) return '3'; break;
                case Keys.NumPad4: if (numLock && !shift) return '4'; break;
                case Keys.NumPad5: if (numLock && !shift) return '5'; break;
                case Keys.NumPad6: if (numLock && !shift) return '6'; break;
                case Keys.NumPad7: if (numLock && !shift) return '7'; break;
                case Keys.NumPad8: if (numLock && !shift) return '8'; break;
                case Keys.NumPad9: if (numLock && !shift) return '9'; break;

                case Keys.OemBackslash: return shift ? '|' : '\\';
                case Keys.OemCloseBrackets: return shift ? '}' : ']';
                case Keys.OemComma: return shift ? '<' : ',';
                case Keys.OemMinus: return shift ? '_' : '-';
                case Keys.OemOpenBrackets: return shift ? '{' : '[';
                case Keys.OemPeriod: return shift ? '>' : '.';
                case Keys.OemPipe: return shift ? '|' : '\\';
                case Keys.OemPlus: return shift ? '+' : '=';
                case Keys.OemQuestion: return shift ? '?' : '/';
                case Keys.OemQuotes: return shift ? '"' : '\'';
                case Keys.OemSemicolon: return shift ? ':' : ';';
                case Keys.OemTilde: return shift ? '~' : '`';
            }

            return (char)0;
        }

        public static char TranslateAlphabetic(char baseChar, bool shift, bool capsLock)
        {
            return (capsLock ^ shift) ? char.ToUpper(baseChar) : baseChar;
        }
    }

    public class InputState
    {
        public KeyboardHandler KeyboardHandler { get; set; }
        public MouseHandler MouseHandler { get; set; }

        public bool LeftMouseClicked { get; protected set; }
        public bool RightMouseClicked { get; protected set; }

        public List<Keys> PressedKeys;

        public InputState()
        {
            KeyboardHandler = new KeyboardHandler();
            MouseHandler = new MouseHandler();

            PressedKeys = new List<Keys>();

            KeyboardHandler.KeyUp += KeyboardHandler_KeyUp;
            KeyboardHandler.KeyDown += KeyboardHandler_KeyDown;
            MouseHandler.MouseButtonUp += MouseHandler_MouseButtonUp;
        }

        public MouseState MouseState
        {
            get { return Mouse.GetState(); }
        }

        public KeyboardState KeyboardState
        {
            get { return Keyboard.GetState(); }
        }

        public void Update(GameTime gameTime)
        {
            LeftMouseClicked = false;
            RightMouseClicked = false;

            PressedKeys.Clear();

            KeyboardHandler.Update(gameTime);
            MouseHandler.Update(gameTime);
        }

        private void KeyboardHandler_KeyUp(object sender, KeyboardEventArgs e)
        {
            PressedKeys.Add(e.PressedKey);
        }

        void KeyboardHandler_KeyDown(object sender, KeyboardEventArgs e)
        {

        }

        private void MouseHandler_MouseButtonUp(object sender, MouseEventArgs e)
        {
            if (e.PressedKey == MouseKey.LeftButton)
            {
                LeftMouseClicked = true;
            }
            if (e.PressedKey == MouseKey.RightButton)
            {
                RightMouseClicked = true;
            }
        }

    }
}