using System;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI
{
    public class ScreenFactory : IScreenFactory
    {
        public GameScreen CreateScreen(Type screenType)
        {
            return Activator.CreateInstance(screenType) as GameScreen;
        }
    }
}