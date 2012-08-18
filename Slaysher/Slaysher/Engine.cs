using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Slaysher.Game;
using Slaysher.Game.Database;
using Slaysher.Game.GUI;
using Slaysher.Game.IO;
using Slaysher.Game.Scenes;
using Slaysher.Game.Settings;

namespace Slaysher
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;

        public string Username { get; set; }

        public string Password { get; set; }

        public GameState GameState { get; set; }

        public GUIManager GUIManager { get; set; }

        public SettingsManager Settings { get; set; }

        private readonly ScreenManager _screenManager;
        private readonly ScreenFactory _screenFactory;

        private readonly KeyboardHandler _keyboardHandler;

        public KeyboardHandler Keyboard
        {
            get { return _keyboardHandler; }
        }

        public Database ClientDatabase { get; set; }

        public Engine()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            Settings = new SettingsManager();

            _screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), _screenFactory);

            _screenManager = new ScreenManager(this);

            Components.Add(_screenManager);
            AddInitialScreens();

            InitGraphicsMode(Settings.GameSettings.ScreenWidth, Settings.GameSettings.ScreenHeight, Settings.GameSettings.Fullscreen);

            _keyboardHandler = new KeyboardHandler();

            GUIManager = new GUIManager(this);
            ClientDatabase = new Database();

            //Set Master Volumes. Replace later with user options
            SoundEffect.MasterVolume = Settings.GameSettings.MasterVolume / 100;
            MediaPlayer.Volume = Settings.GameSettings.MusicVolume / 100;

        }

        private void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScene());
            _screenManager.AddScreen(new MainMenuScene());
        }


        protected override void Initialize()
        {
            //GameState = GameState.Game;
            GUIManager.LoadScene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            //_activeScene.UnloadScene();
        }

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update(gameTime);
            GUIManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        public bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    _graphics.PreferredBackBufferWidth = iWidth;
                    _graphics.PreferredBackBufferHeight = iHeight;
                    _graphics.IsFullScreen = false;
                    _graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                if (
                    GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Any(
                        dm => (dm.Width == iWidth) && (dm.Height == iHeight)))
                {
                    _graphics.PreferredBackBufferWidth = iWidth;
                    _graphics.PreferredBackBufferHeight = iHeight;
                    _graphics.IsFullScreen = true;
                    _graphics.ApplyChanges();
                    return true;
                }
            }
            return false;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Settings.ForceSave();
            base.OnExiting(sender, args);
        }
    }
}