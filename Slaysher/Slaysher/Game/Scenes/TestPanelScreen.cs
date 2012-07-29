using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class TestPanelScreen : WindowScreen
    {
        #region Overrides of WindowScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            PanelEntries.Add(new Button("Test Button 1"));
            PanelEntries.Add(new Button("Test Button 2"));
            PanelEntries.Add(new Button("Test Button 3"));

            Vector2 itemSize = UpdateSubItemPositions();
            PresentationOffset = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2 - itemSize.X/2,
                                             ScreenManager.GraphicsDevice.Viewport.Height/2 - itemSize.Y/2);
        }

        #endregion
    }
}