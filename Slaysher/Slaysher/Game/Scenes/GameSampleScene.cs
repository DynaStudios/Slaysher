using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.World.Objects;
using Slaysher.Graphics.Camera;
using Slaysher.Network;
using SlaysherNetworking.Game.World.Objects;
using SlaysherNetworking.Packets.Utils;

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

        public Dictionary<int, Pattern> Pattern;
        public Dictionary<int, GameObject> GameObjects;

        private Matrix _worldMatrix;

        private Camera _tempCamera = new Camera();
        private Model _patternBaseModel;

        private Dictionary<int, Texture2D> _patternTextures;

        private Texture2D _loadingScreen;
        private volatile bool _contentLoaded;

        //Network Stuff
        Client _client;

        public GameSampleScene(Engine engine)
        {
            Engine = engine;

            Pattern = new Dictionary<int, Pattern>();
            _patternTextures = new Dictionary<int, Texture2D>();
            GameObjects = new Dictionary<int, GameObject>();

            _client = new Client(this);
        }

        private void AsyncLoadScene()
        {
            IPAddress address;

#if DEBUG
            NetworkUtils.Resolve("127.0.0.1", out address);
#else
            NetworkUtils.Resolve("slaysher.dyna-studios.com", out address);
#endif
            IPEndPoint ip = new IPEndPoint(address, 25104);
            Task.Factory.StartNew(() => _client.Start(ip));

            _worldMatrix = Matrix.Identity;
            _patternBaseModel = Engine.Content.Load<Model>("Models/Pattern/Pattern");

            //Pattern testPattern = new Pattern(new Vector3(0, 0, 0), LoadPatternTexture(1));
            //Pattern testPattern2 = new Pattern(new Vector3(50, 0, 50), LoadPatternTexture(2));
            //Pattern.Add(1, testPattern);
            //Pattern.Add(2, testPattern2);

            while (_client.WaitInitialPositionRequest)
            {
                //Wait until Server send us all necassery data.
                Thread.Sleep(10);
            }
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
                foreach (KeyValuePair<int, Pattern> key in Pattern)
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

        public Texture2D LoadPatternTexture(int textureId)
        {
            if (_patternTextures.ContainsKey(textureId))
            {
                return _patternTextures[textureId];
            }
            else
            {
                //Load Pattern Texture into Memory
                //Dirty Translation Map here. Replace with local database
                switch (textureId)
                {
                    case 1:
                        //Dirt
                        _patternTextures.Add(textureId, Engine.Content.Load<Texture2D>("Images/Game/Pattern/dirt"));
                        break;
                    case 2:
                        //Grass
                        _patternTextures.Add(textureId, Engine.Content.Load<Texture2D>("Images/Game/Pattern/grass"));
                        break;
                    case 3:
                        //Sand
                        _patternTextures.Add(textureId, Engine.Content.Load<Texture2D>("Images/Game/Pattern/sand"));
                        break;
                    case 4:
                        //Water
                        _patternTextures.Add(textureId, Engine.Content.Load<Texture2D>("Images/Game/Pattern/water"));
                        break;
                }
                return _patternTextures[textureId];
            }
        }

        public void UnloadScene()
        {
            _client.Dispose();
        }
    }
}