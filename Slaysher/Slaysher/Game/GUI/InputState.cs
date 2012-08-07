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
            KeyboardState keyboard = Keyboard.GetState();
            StringBuilder stringBuilder = new StringBuilder(text);
            cursorPosition = (cursorPosition != -1) ? cursorPosition : stringBuilder.Length - 1;

            foreach (Keys key in keys)
            {
                if (key == Keys.LeftShift)
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
                if (key == Keys.Space)
                {
                    stringBuilder.Insert(cursorPosition + 1, " ");
                    continue;
                }

                bool shiftPressed = (keyboard.IsKeyDown(Keys.LeftShift));

                var inputText = (shiftPressed) ? key.ToString() : key.ToString().ToLower();

                stringBuilder.Insert(cursorPosition + 1, inputText);
                cursorPosition++;

            }

            return stringBuilder.ToString();
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