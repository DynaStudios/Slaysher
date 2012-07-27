using Microsoft.Xna.Framework.Input;

namespace Slaysher.Game.GUI
{
    public class InputState
    {
        public MouseState MouseState
        {
            get { return Mouse.GetState(); }
        }
        public KeyboardState KeyboardState { get; protected set; }

        public InputState()
        {
            KeyboardState = Keyboard.GetState();
        }
    }
}