using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Slaysher.Game.Scenes
{
    public interface IScene
    {
        void LoadScene();

        void Render(GameTime time);

        void Update(GameTime time);

        void UnloadScene();
    }
}