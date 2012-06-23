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
            menu = new MenuComponent(Engine, _menuItems, new SpriteBatch(Engine.GraphicsDevice), new Vector2(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth - 300, Engine.GraphicsDevice.PresentationParameters.BackBufferWidth - 550));
            Engine.Components.Add(menu);

            menu.MouseClick += mouse_MouseClick;

            menu.Enabled = true;
            menu.Visible = true;
        }

        public void mouse_MouseClick(object sender, EventArgs eventArgs)
        {
            MenuSelectEvent eventA = (MenuSelectEvent)eventArgs;

            switch (eventA.Selected)
            {
                case 0:
                    //New Game
                    break;
                case 1:
                    //Servers
                    break;
                case 2:
                    //Options
                    break;
                case 3:
                    Engine.Exit();
                    break;
            }
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void UnloadScene()
        {
            menu.MouseClick -= mouse_MouseClick;
            Engine.Components.Remove(menu);
        }
    }
}