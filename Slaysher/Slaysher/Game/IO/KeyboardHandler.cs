using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Slaysher.Game.IO
{
    public class KeyboardHandler
    {
        public event EventHandler KeyDown;

        public event EventHandler KeyUp;

        private readonly Dictionary<Keys, bool> _pressedKeys;

        public KeyboardHandler()
        {
            _pressedKeys = new Dictionary<Keys, bool>();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyBoardState = Keyboard.GetState();
            foreach (Keys key in keyBoardState.GetPressedKeys())
            {
                if (!_pressedKeys.ContainsKey(key))
                {
                    _pressedKeys.Add(key, true);
                    if (KeyDown != null)
                    {
                        KeyDown(this, new KeyboardEventArgs(key));
                    }
                }
            }

            List<Keys> keysToDelete = new List<Keys>();
            foreach (KeyValuePair<Keys, bool> key in _pressedKeys)
            {
                if (!keyBoardState.IsKeyDown(key.Key))
                {
                    keysToDelete.Add(key.Key);
                    if (KeyUp != null)
                    {
                        KeyUp(this, new KeyboardEventArgs(key.Key));
                    }
                }
            }

            foreach (Keys key in keysToDelete)
            {
                _pressedKeys.Remove(key);
            }
        }
    }
}