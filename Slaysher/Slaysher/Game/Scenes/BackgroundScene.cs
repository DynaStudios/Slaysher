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

         public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
         {
             base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
         }
    }
}