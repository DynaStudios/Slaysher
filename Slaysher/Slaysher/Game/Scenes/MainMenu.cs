using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Slaysher.Game.GUI.Menu;

namespace Slaysher.Game.Scenes
{
    public class MainMenu : IScene
    {
        public string Name
        {
            get { return "mainMenu"; }
        }

        public Engine Engine { get; set; }

        private readonly string[] _menuItems;

        private MenuComponent _menu;

        public MainMenu(Engine engine)
        {
            Engine = engine;

            _menuItems = new[] { "Game Test", "Servers", "Options", "Exit Game" };
        }

        private MenuComponent CreateMenuComponent()
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
            _menu = CreateMenuComponent();
            Engine.Components.Add(_menu);

            _menu.MouseClick += MouseMouseClick;

            Song mainMenuMusic = Engine.Content.Load<Song>("Sounds/Music/rainbow");
            MediaPlayer.Play(mainMenuMusic);
        }

        public void MouseMouseClick(object sender, EventArgs eventArgs)
        {
            MenuSelectEvent eventA = (MenuSelectEvent)eventArgs;

            string selected = _menuItems[eventA.Selected];

            switch (selected)
            {
                case "Servers":
                    //Servers
                    break;
                case "Options":
                    //Options
                    break;
                case "Exit Game":
                    Engine.Exit();
                    break;
                case "Game Test":
                    Engine.SwitchScene("gameScene");
                    break;
            }
        }

        public void Render(GameTime time)
        {
            if (Engine.GameState == GameState.Game)
            {
                _menu.Enabled = true;
                _menu.Visible = true;
            }
            else
            {
                _menu.Enabled = false;
                _menu.Visible = false;
            }
        }

        public void Update(GameTime time)
        {
        }

        public void UnloadScene()
        {
            _menu.MouseClick -= MouseMouseClick;
            Engine.Components.Remove(_menu);
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Pause();

            Engine.Content.Unload();
        }
    }
}