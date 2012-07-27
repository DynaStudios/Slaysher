using System;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI
{
    public interface IScreenFactory
    {
        GameScreen CreateScreen(Type screenType);
    }
}