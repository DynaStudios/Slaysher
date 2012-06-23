using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.IO;

namespace Slaysher.Game.Scenes
{
    public class SplashScreen : IScene
    {
        private static int SPLASH_TIME = 2;

        public Engine Engine { get; set; }

        private SpriteBatch _spriteBatch;
        private Texture2D _splashScreen;

        private TimeSpan startTime;

        public SplashScreen(Engine engine)
        {
            Engine = engine;
        }

        public void LoadScene()
        {
            _spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            _splashScreen = Engine.Content.Load<Texture2D>("Images/Game/dyna_splash");

            Engine.Keyboard.KeyUp += keyboard_KeyUp;
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
            if (startTime == null)
            {
                startTime = time.TotalGameTime;
            }

            _spriteBatch.Begin();
            _spriteBatch.Draw(_splashScreen, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            if (time.TotalGameTime.Seconds == SplashScreen.SPLASH_TIME)
            {
                //Engine.SwitchScene("boxTest");
                Engine.SwitchScene("mainMenu");
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        private void keyboard_KeyUp(object sender, EventArgs eventArgs)
        {
            KeyboardEventArgs eventA = (KeyboardEventArgs)eventArgs;
            if (eventA.PressedKey == Keys.Escape)
            {
                Engine.SwitchScene("mainMenu");
            }
        }

        public void UnloadScene()
        {
            _spriteBatch = null;
            _splashScreen = null;

            Engine.Keyboard.KeyUp -= keyboard_KeyUp;
        }
    }
}