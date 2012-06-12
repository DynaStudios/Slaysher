using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Slaysher.Game.Scenes;
using Slaysher.Graphics.Camera;

namespace Slaysher
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Dictionary<String, IScene> _availableScenes;
        private IScene _activeScene;
        private bool _sceneLoaded;
        private string _sceneSwitchName;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _availableScenes = new Dictionary<string, IScene>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Load SplashScreen as Sample
            IScene splashScreen = new SplashScreen(this);
            IScene boxTest = new BoxSampleScene(this);

            //Add Scene to List
            AddScene("splashScreen", splashScreen);
            AddScene("boxTest", boxTest);

            //Switch to chosen Scene
            SwitchScene("boxTest");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_activeScene != null)
            {
                _activeScene.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
            if (_activeScene == null && _availableScenes.ContainsKey(sceneName))
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
    }
}