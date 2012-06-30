using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.World.Objects;
using Slaysher.Graphics.Camera;

namespace Slaysher.Game.Scenes
{
    public class GameSampleScene : IScene
    {
        public string Name
        {
            get { return "gameScene"; }
        }

        public Engine Engine { get; set; }

        private SpriteBatch _spriteBatch;

        private Dictionary<int, Pattern> _patterns;
        private Dictionary<int, GameObject> _gameObjects;

        private Matrix _worldMatrix;

        private Camera _tempCamera = new Camera();
        private Model _patternBaseModel;

        private Texture2D _loadingScreen;
        private volatile bool _contentLoaded;

        public GameSampleScene(Engine engine)
        {
            Engine = engine;

            _patterns = new Dictionary<int, Pattern>();
            _gameObjects = new Dictionary<int, GameObject>();
        }

        private void AsyncLoadScene()
        {
            _worldMatrix = Matrix.Identity;
            _patternBaseModel = Engine.Content.Load<Model>("Models/Pattern/Pattern");

            Pattern testPattern = new Pattern(new Vector3(0, 0, 0));
            Pattern testPattern2 = new Pattern(new Vector3(50, 0, 0));
            _patterns.Add(1, testPattern);
            _patterns.Add(2, testPattern2);

            Thread.Sleep(2000);
            _contentLoaded = true;
        }

        public void LoadScene()
        {
            _spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            _loadingScreen = Engine.Content.Load<Texture2D>("Images/Game/loadingScreen");
            ThreadPool.QueueUserWorkItem(delegate { AsyncLoadScene(); });
        }

        public void Render(GameTime time)
        {
            if (_contentLoaded)
            {
                foreach (KeyValuePair<int, Pattern> key in _patterns)
                {
                    key.Value.Draw(_patternBaseModel, _worldMatrix, _tempCamera);
                }
            }
            else
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_loadingScreen, new Vector2(0, 0), Color.White);
                _spriteBatch.End();
            }
        }

        public void Update(GameTime time)
        {
            KeyboardState keyBoardState = Keyboard.GetState();

            //Rotate Cube along its Up Vector
            if (keyBoardState.IsKeyDown(Keys.X))
            {
                _worldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, .02f) * _worldMatrix;
            }
            if (keyBoardState.IsKeyDown(Keys.Z))
            {
                _worldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, -.02f) * _worldMatrix;
            }

            //Move Cube Forward, Back, Left, and Right
            if (keyBoardState.IsKeyDown(Keys.Up))
            {
                _worldMatrix *= Matrix.CreateTranslation(_worldMatrix.Forward);
            }
            if (keyBoardState.IsKeyDown(Keys.Down))
            {
                _worldMatrix *= Matrix.CreateTranslation(_worldMatrix.Backward);
            }
            if (keyBoardState.IsKeyDown(Keys.Left))
            {
                _worldMatrix *= Matrix.CreateTranslation(-_worldMatrix.Right);
            }
            if (keyBoardState.IsKeyDown(Keys.Right))
            {
                _worldMatrix *= Matrix.CreateTranslation(_worldMatrix.Right);
            }

            _tempCamera.Update(_worldMatrix);
        }

        public void UnloadScene()
        {
        }
    }
}