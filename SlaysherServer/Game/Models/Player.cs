using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlaysherServer.Game.Models
{
    public class Player
    {
        public int Health { get; set; }

        public WorldPosition Position { get; set; }

        public string Nickname { get; set; }

        public Player() { }
    }
}