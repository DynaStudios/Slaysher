using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Slaysher.Game;
using Slaysher.Game.GUI;
using Slaysher.Game.IO;
using Slaysher.Game.Scenes;

namespace Slaysher
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Dictionary<String, IScene> _availableScenes;
        private IScene _activeScene;
        private bool _sceneLoaded;
        private string _sceneSwitchName;

        public GameState GameState { get; set; }

        public GUIManager GUIManager { get; set; }

        private KeyboardHandler _keyboardHandler;

        public KeyboardHandler Keyboard { get { return _keyboardHandler; } }

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            InitGraphicsMode(1024, 768, false);

            _availableScenes = new Dictionary<string, IScene>();
            _keyboardHandler = new KeyboardHandler();

            GUIManager = new GUIManager(this);

            //Set Master Volumes. Replace later with user options
            SoundEffect.MasterVolume = 0.3f;
            MediaPlayer.Volume = 0.3f;
        }

        protected override void Initialize()
        {
            //Load SplashScreen as Sample
            IScene splashScreen = new SplashScreen(this);
            IScene boxTest = new BoxSampleScene(this);
            IScene gameScene = new GameSampleScene(this);
            IScene mainMenu = new MainMenu(this);

            //Add Scene to List
            AddScene("splashScreen", splashScreen);
            AddScene("boxTest", boxTest);
            AddScene("gameScene", gameScene);
            AddScene("mainMenu", mainMenu);

            //Switch to chosen Scene
            SwitchScene("splashScreen");

            GameState = GameState.GAME;
            GUIManager.LoadScene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (_activeScene != null)
            {
                _activeScene.Update(gameTime);
            }

            Keyboard.Update(gameTime);
            GUIManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            //Scene Rendering
            if (!_sceneLoaded)
            {
                loadScene();
            }

            if (_activeScene != null)
            {
                _activeScene.Render(gameTime);
            }

            GUIManager.Render(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Adds Scene to availbale Scene List
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        /// <param name="scene">IScene Implementation</param>
        public void AddScene(String sceneName, IScene scene)
        {
            if (!_availableScenes.ContainsKey(sceneName))
            {
                _availableScenes.Add(sceneName, scene);
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

        private void loadScene()
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

        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}