using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Slaysher.Game.Physics.Collisions;

namespace Slaysher.Game.GUI.Menu
{
    public class MenuEntry : Box
    {
        public string Text { get; set; }

        public MenuEntry(string text, Vector2 position, Vector2 size)
            : base(position, size)
        {
            Text = text;
        }
    }
}