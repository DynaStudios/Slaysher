using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Slaysher.Game.GUI.Menu;
using Slaysher.Game.IO;

namespace Slaysher.Game.Scenes
{
    public class MainMenu : IScene
    {
        public string Name
        {
            get { return "mainMenu"; }
        }

        public Engine Engine { get; set; }

        private string[] _menuItems;
        private int _selectedItem;

        private MenuComponent menu;

        public MainMenu(Engine engine)
        {
            Engine = engine;

            _selectedItem = 0;
            _menuItems = new string[] { "Box Test", "Pattern Test", "Game Test", "Servers", "Options", "Exit Game" };
        }

        private MenuComponent createMenuComponent()
        {
            SpriteBatch spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            float backBufferWidth = Engine.GraphicsDevice.PresentationParameters.BackBufferWidth;
            float x = backBufferWidth - 300;
            float y = backBufferWidth - 550;
            Vector2 vector = new Vector2(x, y);

            return new MenuComponent(Engine, _menuItems, spriteBatch, vector);
        }

        public void LoadScene()
        {
            menu = createMenuComponent();
            Engine.Components.Add(menu);

            menu.MouseClick += mouse_MouseClick;

            Song mainMenuMusic = Engine.Content.Load<Song>("Sounds/Music/rainbow");
            MediaPlayer.Play(mainMenuMusic);
        }

        public void mouse_MouseClick(object sender, EventArgs eventArgs)
        {
            MenuSelectEvent eventA = (MenuSelectEvent)eventArgs;

            string selected = _menuItems[eventA.Selected];

            switch (selected)
            {
                case "Box Test":
                    Engine.SwitchScene("boxTest");
                    break;
                case "Servers":
                    //Servers
                    break;
                case "Options":
                    //Options
                    break;
                case "Exit Game":
                    Engine.Exit();
                    break;
                case "Pattern Test":
                    break;
                case "Game Test":
                    Engine.SwitchScene("gameScene");
                    break;
            }
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
            if (Engine.GameState == GameState.GAME)
            {
                menu.Enabled = true;
                menu.Visible = true;
            }
            else
            {
                menu.Enabled = false;
                menu.Visible = false;
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void UnloadScene()
        {
            menu.MouseClick -= mouse_MouseClick;
            Engine.Components.Remove(menu);
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Pause();

            Engine.Content.Unload();
        }
    }
}