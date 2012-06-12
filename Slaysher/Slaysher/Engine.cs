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
        Camera camera = new Camera();

        Matrix cubeWorld;
        Model cubeModel;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private SpriteFont font;
        private bool cameraInfoActivated = false;

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
            // TODO: Add your initialization logic here
            cubeWorld = Matrix.Identity;

            font = Content.Load<SpriteFont>("Fonts/Main");

            //Load SplashScreen as Sample
            IScene splashScreen = new SplashScreen(this);

            //Add Scene to List
            AddScene("splashScreen", splashScreen);

            //Switch to chosen Scene
            SwitchScene("splashScreen");

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

            // TODO: use this.Content to load your game content here
            cubeModel = Content.Load<Model>("Cube");
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
            KeyboardState keyBoardState = Keyboard.GetState();

            //Rotate Cube along its Up Vector
            if (keyBoardState.IsKeyDown(Keys.X))
            {
                cubeWorld = Matrix.CreateFromAxisAngle(Vector3.Up, .02f) * cubeWorld;
            }
            if (keyBoardState.IsKeyDown(Keys.Z))
            {
                cubeWorld = Matrix.CreateFromAxisAngle(Vector3.Up, -.02f) * cubeWorld;
            }

            //Move Cube Forward, Back, Left, and Right
            if (keyBoardState.IsKeyDown(Keys.Up))
            {
                cubeWorld *= Matrix.CreateTranslation(cubeWorld.Forward);
            }
            if (keyBoardState.IsKeyDown(Keys.Down))
            {
                cubeWorld *= Matrix.CreateTranslation(cubeWorld.Backward);
            }
            if (keyBoardState.IsKeyDown(Keys.Left))
            {
                cubeWorld *= Matrix.CreateTranslation(-cubeWorld.Right);
            }
            if (keyBoardState.IsKeyDown(Keys.Right))
            {
                cubeWorld *= Matrix.CreateTranslation(cubeWorld.Right);
            }
            if (keyBoardState.IsKeyDown(Keys.F5))
            {
                //Activate Camera Info
                if (cameraInfoActivated)
                {
                    cameraInfoActivated = false;
                }
                else
                {
                    cameraInfoActivated = true;
                }
            }

            if (_activeScene != null)
            {
                _activeScene.Update(gameTime);
            }

            // TODO: Add your update logic here
            camera.Update();

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

            // TODO: Add your drawing code here
            DrawModel(cubeModel, cubeWorld);

            if (cameraInfoActivated)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Yaw: " + camera.yaw + ", Pitch: " + camera.pitch, Vector2.Zero, Color.Red);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix worldMatrix)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projectionMatrix;
                }
                mesh.Draw();
            }
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