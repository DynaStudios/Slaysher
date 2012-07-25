﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Slaysher.Game.IO;

namespace Slaysher.Game.Scenes
{
    public class SplashScreen : IScene
    {
        public string Name
        {
            get { return "splashScreen"; }
        }

        private const int SplashTime = 4;

        public Engine Engine { get; set; }

        private SpriteBatch _spriteBatch;
        private Texture2D _splashScreen;

        private Song _splashSoundfile;

        private TimeSpan _startTime;

        public SplashScreen(Engine engine)
        {
            Engine = engine;
        }

        public void LoadScene()
        {
            _spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            _splashScreen = Engine.Content.Load<Texture2D>("Images/Game/dyna_splash");
            _splashSoundfile = Engine.Content.Load<Song>("Sounds/Splash/splashSound");

            MediaPlayer.Play(_splashSoundfile);

            Engine.Keyboard.KeyUp += KeyboardKeyUp;
        }

        public void Render(GameTime time)
        {
            if (_startTime == null)
            {
                _startTime = time.TotalGameTime;
            }

            _spriteBatch.Begin();
            _spriteBatch.Draw(_splashScreen, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            if (time.TotalGameTime.Seconds == SplashTime)
            {
                //Engine.SwitchScene("boxTest");
                Engine.SwitchScene("mainMenu");
            }
        }

        public void Update(GameTime time)
        {
        }

        private void KeyboardKeyUp(object sender, EventArgs eventArgs)
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

            MediaPlayer.Pause();
            Engine.Keyboard.KeyUp -= KeyboardKeyUp;

            Engine.Content.Unload();
        }
    }
}