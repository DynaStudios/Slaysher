using System;

namespace Slaysher.Game.GUI.Menu
{
    public class MenuSelectEvent : EventArgs
    {
        public int Selected { get; set; }

        public MenuSelectEvent(int value)
        {
            Selected = value;
        }
    }
}