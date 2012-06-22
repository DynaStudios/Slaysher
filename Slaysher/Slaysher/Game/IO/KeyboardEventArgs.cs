using System;
using Microsoft.Xna.Framework.Input;

namespace Slaysher.Game.IO
{
    public class KeyboardEventArgs : EventArgs
    {
        private Keys _pressedKey;

        public Keys PressedKey { get { return _pressedKey; } }

        public KeyboardEventArgs(Keys key)
        {
            _pressedKey = key;
        }
    }
}