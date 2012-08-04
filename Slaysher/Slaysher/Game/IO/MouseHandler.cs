using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Slaysher.Game.IO
{
    public class MouseHandler
    {
        public event EventHandler<MouseEventArgs> MouseButtonUp;
        public event EventHandler<MouseEventArgs> MouseButtonDown;

        private bool _leftMouseButtonIsDown;
        private bool _rightMouseButtonIsDown;

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!_leftMouseButtonIsDown)
                {
                    _leftMouseButtonIsDown = true;
                    if (MouseButtonDown != null)
                    {
                        MouseButtonDown(this, new MouseEventArgs(MouseKey.LeftButton));
                    }
                }
            }
            else
            {
                if (_leftMouseButtonIsDown)
                {
                    _leftMouseButtonIsDown = false;
                    if (MouseButtonUp != null)
                    {
                        MouseButtonUp(this, new MouseEventArgs(MouseKey.LeftButton));
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (!_rightMouseButtonIsDown)
                {
                    _rightMouseButtonIsDown = true;
                    if (MouseButtonDown != null)
                    {
                        MouseButtonDown(this, new MouseEventArgs(MouseKey.RightButton));
                    }
                }
            }
            else
            {
                if (_rightMouseButtonIsDown)
                {
                    _rightMouseButtonIsDown = false;
                    if (MouseButtonUp != null)
                    {
                        MouseButtonUp(this, new MouseEventArgs(MouseKey.RightButton));
                    }
                }
            }
        }

    }
}