using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes.WorldGen
{
    public class WorldGenMenu : WindowScreen
    {
        #region Overrides of GameScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            PresentationOffset = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 4) * 3f, (ScreenManager.GraphicsDevice.Viewport.Height / 4) * 3f);
        }

        #endregion

        #region Overrides of WindowScreen

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion
    }
}