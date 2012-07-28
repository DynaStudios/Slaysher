using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class BackgroundScene : GameScreen
    {
        private ContentManager _content;
        private Texture2D _backgroundTexture;

         public BackgroundScene()
         {
             TransitionOnTime = TimeSpan.FromSeconds(0.5);
             TransitionOffTime = TimeSpan.FromSeconds(0.5);
         }

        /// <summary>
        /// Activates the screen. This method gets called after the screen is added to the screen manager.
        /// </summary>
        /// <param name="instancePreserved">Not used yet! Will be used later for serialization</param>
        public override void Activate(bool instancePreserved)
         {
             if (!instancePreserved)
             {
                 if (_content == null)
                 {
                     _content = new ContentManager(ScreenManager.Game.Services, "Content");
                 }

                 _backgroundTexture = _content.Load<Texture2D>("Images/Game/loadingScreen");
             }
         }

        /// <summary>
        /// Unloads screen content.
        /// </summary>
        public override void Unload()
         {
             _content.Unload();
         }

         public override void Draw(GameTime gameTime)
         {
             SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
             Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
             Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

             spriteBatch.Begin();

             spriteBatch.Draw(_backgroundTexture, fullscreen, Color.White);

             spriteBatch.End();
         }

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
             base.Update(gameTime, otherScreenHasFocus, false);
         }
    }
}