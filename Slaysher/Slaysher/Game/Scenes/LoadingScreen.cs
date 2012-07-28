using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class LoadingScreen : GameScreen
    {

        private bool _loadingIsSlow;
        private bool _otherScreensAreGone;

        private GameScreen[] _screensToLoad;

        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(ScreenManager screenManager, bool loadingIsSlow, params GameScreen[] screensToLoad)
        {
            foreach (GameScreen gameScreen in screenManager.GetScreens())
            {
                gameScreen.ExitScreen();

                LoadingScreen loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
                screenManager.AddScreen(loadingScreen);
            }
        }

        #region Overrides of GameScreen

        /// <summary>
        /// This method gets called with every update call. All screens get called! Even the not visible one.
        /// 
        /// To update just the visible one use HandleInput()
        /// </summary>
        /// <param name="gameTime">Current GameTime</param>
        /// <param name="otherScreenHasFocus">Is another screen having focus?</param>
        /// <param name="coveredByOtherScreen">Depending on covered or not the screen will be visible. Pass true to always be visible in the background</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);
                foreach (GameScreen gameScreen in _screensToLoad)
                {
                    if (gameScreen != null)
                    {
                        ScreenManager.AddScreen(gameScreen);
                    }
                }

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) && (ScreenManager.GetScreens().Length == 1))
            {
                _otherScreensAreGone = true;
            }

            if (_loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;

                const string message = "Loading...";

                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize)/2;

                Color color = Color.White*TransitionAlpha;

                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }

            #endregion
        }
    }
}