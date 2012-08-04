using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Slaysher.Game.Entities;
using Slaysher.Game.World.Objects;
using Slaysher.Graphics.Camera;
using Slaysher.Network;

using SlaysherNetworking.Game.World.Objects;
using SlaysherNetworking.Packets.Utils;

namespace Slaysher.Game.Scenes
{
    public partial class GameScene : IScene
    {
        public string Name
        {
            get { return "gameScene"; }
        }

        public Engine Engine { get; set; }

        private SpriteBatch _spriteBatch;

        public Dictionary<int, Pattern> Pattern;
        public Dictionary<int, GameObject> GameObjects;
        public ClientPlayer Player { get; set; }

        private Matrix _worldMatrix;

        private readonly Camera _tempCamera = new Camera(new SlaysherNetworking.Game.World.WorldPosition());
        public Camera Camera { get { return _tempCamera; } }
        private Model _patternBaseModel;

        private readonly Dictionary<int, Texture2D> _patternTextures;
        private Dictionary<int, string> _availablePatternTextures;

        private Texture2D _loadingScreen;
        private volatile bool _contentLoaded;

        //Network Stuff
        private readonly Client _client;

        public GameScene(Engine engine)
        {
            Engine = engine;

            Pattern = new Dictionary<int, Pattern>();
            _patternTextures = new Dictionary<int, Texture2D>();
            GameObjects = new Dictionary<int, GameObject>();

            _client = new Client(this);
        }

        private void AsyncLoadScene()
        {
            _availablePatternTextures = Engine.ClientDatabase.ReadAvailablePatternTextures();
            _availablePatternTextures.Add(0, "Images/Game/Pattern/missing");

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

            _client.WaitForInitialPositionRequest();
            Player.LoadPlayerModel(Engine.Content);
            Camera.Target = Player.VisualPosition;

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
                TickPlayer(time);
                TickWorld(time);

                foreach (KeyValuePair<int, Pattern> key in Pattern)
                {
                    key.Value.Draw(_patternBaseModel, _worldMatrix, _tempCamera);
                }
                Player.Render(Camera);
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
                _worldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, .02f)*_worldMatrix;
            }
            if (keyBoardState.IsKeyDown(Keys.Z))
            {
                _worldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, -.02f)*_worldMatrix;
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
            if (Player != null)
            {
                Player.Update(_worldMatrix);
            }
        }

        public Texture2D LoadPatternTexture(int textureId)
        {
            if (_patternTextures.ContainsKey(textureId))
            {
                return _patternTextures[textureId];
            }
            //Load Pattern Texture into Memory
            _patternTextures.Add(textureId,
                                 _availablePatternTextures.ContainsKey(textureId)
                                     ? Engine.Content.Load<Texture2D>(_availablePatternTextures[textureId])
                                     : Engine.Content.Load<Texture2D>(_availablePatternTextures[0]));
            return _patternTextures[textureId];
        }

        public void UnloadScene()
        {
            _client.Dispose();
        }
    }
}