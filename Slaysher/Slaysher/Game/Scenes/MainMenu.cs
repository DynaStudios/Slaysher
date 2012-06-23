using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.GUI.Menu;
using Slaysher.Game.IO;

namespace Slaysher.Game.Scenes
{
    public class MainMenu : IScene
    {
        public Engine Engine { get; set; }

        private string[] _menuItems;
        private int _selectedItem;

        private MenuComponent menu;

        public MainMenu(Engine engine)
        {
            Engine = engine;

            _selectedItem = 0;
            _menuItems = new string[] { "New World", "Servers", "Options", "Exit Game" };
        }

        public void LoadScene()
        {
            Engine.Keyboard.KeyUp += keyboard_KeyUp;

            menu = new MenuComponent(Engine, _menuItems, new SpriteBatch(Engine.GraphicsDevice), new Vector2(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth - 300, Engine.GraphicsDevice.PresentationParameters.BackBufferWidth - 550));
            Engine.Components.Add(menu);

            menu.Enabled = true;
            menu.Visible = true;
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        private void keyboard_KeyUp(object sender, EventArgs eventArgs)
        {
            KeyboardEventArgs eventA = (KeyboardEventArgs)eventArgs;

            switch (eventA.PressedKey)
            {
                case Keys.Up:
                    //Handle KeyPress "UP"
                    break;
                case Keys.Down:
                    //Handle KeyPress "DOWN"
                    break;
                case Keys.Enter:
                    //Handle KeyPress "Enter"
                    break;
            }
        }

        public void UnloadScene()
        {
            Engine.Keyboard.KeyUp -= keyboard_KeyUp;
        }
    }
}