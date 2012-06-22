using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Slaysher.Game.Scenes
{
    public class SplashScreen : IScene
    {
        public Engine Engine { get; set; }

        private SpriteBatch _spriteBatch;
        private Texture2D _splashScreen;

        public SplashScreen(Engine engine)
        {
            Engine = engine;
        }

        public void LoadScene()
        {
            _spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            _splashScreen = Engine.Content.Load<Texture2D>("Images/Game/dyna_splash");
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_splashScreen, new Vector2(0, 0), Color.White);
            _spriteBatch.End();
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void UnloadScene()
        {
        }
    }
}