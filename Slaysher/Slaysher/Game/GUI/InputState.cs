using System;
using System.Collections.Generic;
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