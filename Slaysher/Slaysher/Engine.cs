using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Slaysher.Exceptions;
using Slaysher.Game;
using Slaysher.Game.Database;
using Slaysher.Game.GUI;
using Slaysher.Game.IO;
using Slaysher.Game.Scenes;

namespace Slaysher
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private readonly Dictionary<String, IScene> _availableScenes;
        private IScene _activeScene;
        private bool _sceneLoaded;
        private string _sceneSwitchName;

        public string Username { get; set; }

        public string Password { get; set; }

        public GameState GameState { get; set; }

        public GUIManager GUIManager { get; set; }

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
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            _screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), _screenFactory);

            _screenManager = new ScreenManager(this);
            _screenManager.TraceEnabled = true;

            Components.Add(_screenManager);
            AddInitialScreens();

            InitGraphicsMode(1024, 768, false);

            _availableScenes = new Dictionary<string, IScene>();
            _keyboardHandler = new KeyboardHandler();

            GUIManager = new GUIManager(this);
            ClientDatabase = new Database();

            //Set Master Volumes. Replace later with user options
            SoundEffect.MasterVolume = 0.3f;
            MediaPlayer.Volume = 0.3f;

        }

        private void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScene());
            _screenManager.AddScreen(new MainMenuScene());
        }

        private static bool TypeIsScene(Type type)
        {
            if (type.IsClass)
            {
                Type sceneType = typeof (IScene);
                Type[] interfaces = type.GetInterfaces();

                return interfaces.Any(i => sceneType == i);
            }
            return false;
        }

        public void LoadScenesFromAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (TypeIsScene(type))
                {
                    IScene scene = (IScene) Activator.CreateInstance(type, this);
                    AddScene(scene);
                }
            }
        }

        protected override void Initialize()
        {
            //LoadScenesFromAssembly(Assembly.GetExecutingAssembly());

            //Switch to chosen Scene
#if DEBUG
            //SwitchScene("mainMenu");
#else
            SwitchScene("splashScreen");
#endif

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
            /*if (_activeScene != null)
            {
                _activeScene.Update(gameTime);
            }*/

            Keyboard.Update(gameTime);
            GUIManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Scene Rendering
            /*if (!_sceneLoaded)
            {
                LoadScene();
            }

            if (_activeScene != null)
            {
                _activeScene.Render(gameTime);
            }

            GUIManager.Render(gameTime);*/
            base.Draw(gameTime);
        }

        /// <summary>
        /// Adds Scene to availbale Scene List
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        /// <param name="scene">IScene Implementation</param>
        public void AddScene(IScene scene)
        {
            if (!_availableScenes.ContainsKey(scene.Name))
            {
                _availableScenes.Add(scene.Name, scene);
            }
            else
            {
                throw new SceneException("Scenename already exists");
            }
        }

        /// <summary>
        /// Switches to given IScene
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        public void SwitchScene(String sceneName)
        {
            if (_availableScenes.ContainsKey(sceneName))
            {
                _sceneSwitchName = sceneName;
                _sceneLoaded = false;
            }
            else
            {
                throw new SceneException("Scene were not found!");
            }
        }

        private void LoadScene()
        {
            //Unload old scene
            if (_activeScene != null)
            {
                _activeScene.UnloadScene();
            }

            //Set new Scene
            _activeScene = _availableScenes[_sceneSwitchName];

            //Load new Scene
            _activeScene.LoadScene();
            _sceneLoaded = true;
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
    }
}