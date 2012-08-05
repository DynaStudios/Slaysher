using System;

namespace Slaysher.Game.IO
{
    public class MouseEventArgs : EventArgs
    {
        public MouseKey PressedKey;

        public MouseEventArgs(MouseKey mouseKey)
        {
            PressedKey = mouseKey;
        }
    }
}
