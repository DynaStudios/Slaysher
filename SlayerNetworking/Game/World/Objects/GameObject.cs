using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlaysherNetworking.Game.World.Objects
{
    public class GameObject
    {
        public int Id;
        public float PosX { get; set;}
        public float PosY { get; set;}
        public float PosZ { get; set; }
        public float Direction { get; set; }
        public string Model { get; set; }

        public GameObject()
        {

        }
    }
}