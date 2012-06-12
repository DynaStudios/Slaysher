using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slaysher.Game.Scenes
{
    public class SplashScreen : IScene
    {
        public Engine Engine { get; set; }

        public SplashScreen(Engine engine)
        {
            Engine = engine;
        }

        public void LoadScene()
        {
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void UnloadScene()
        {
        }
    }
}