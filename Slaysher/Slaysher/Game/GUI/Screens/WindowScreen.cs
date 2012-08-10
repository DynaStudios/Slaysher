using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Components;

namespace Slaysher.Game.GUI.Screens
{
    public class WindowScreen : GameScreen
    {
        public string Title { get; set; }
        public List<IGuiItem> PanelEntries { get; protected set; }

        public Vector2 PresentationOffset { get; set; }
        public int ItemOffset { get; set; }
        public int ItemPadding { get; set; }

        private Texture2D _windowBackground;

        public WindowScreen()
        {
            PanelEntries = new List<IGuiItem>();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //A Panel is always on top of other screens
            IsPopup = true;
        }

        #region Overrides of GameScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            PresentationOffset = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2,
                                             ScreenManager.GraphicsDevice.Viewport.Height/2);
            ItemOffset = 10;
            ItemPadding = 20;

            _windowBackground = ScreenManager.Game.Content.Load<Texture2D>("Images/Game/Menu/btnBg");
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            foreach (IGuiItem item in PanelEntries)
            {
                item.HandleInput(input);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 size = UpdateSubItemPositions();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            //Draw Rectangle only of Tranition finished
            if (ScreenState == ScreenState.Active) { 
                Rectangle rec = new Rectangle((int) PresentationOffset.X, (int) PresentationOffset.Y,
                                              (int) size.X + ItemPadding*2, (int) size.Y + ItemPadding*3);

                spriteBatch.Draw(_windowBackground, rec, Color.Black*0.5f);
            }

            var zSortedList = (from a in PanelEntries orderby a.ZIndex ascending select a).ToArray();

            //Loop Items here
            foreach (IGuiItem item in zSortedList)
            {
                item.Draw(this, gameTime);
            }

            spriteBatch.End();
        }

        #endregion

        /// <summary>
        /// Calculates Position for every Panel Item
        /// </summary>
        /// <returns>Whole Size</returns>
        public Vector2 UpdateSubItemPositions()
        {
            float transitionOffset = (float) Math.Pow(TransitionPosition, 2);

            Vector2 position = PresentationOffset;

            float maxX = 0;
            int offsetCount = 0;
            foreach (IGuiItem guiItem in PanelEntries)
            {
                var itemWidth = guiItem.GetWidth(this);
                if (itemWidth > maxX)
                {
                    maxX = itemWidth;
                }
                position.X = PresentationOffset.X + ItemPadding;

                var yOffset = (offsetCount > 0) ? ItemOffset : 0;
                var startY = (offsetCount > 0) ? PanelEntries[offsetCount - 1].GetHeight(this) : ItemPadding;
                position.Y += startY + yOffset;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset*256;
                else
                    position.X += transitionOffset*512;

                guiItem.Position = position;
                offsetCount++;
            }

            var width = maxX;
            var height = PresentationOffset.Y - position.Y;

            return new Vector2(Math.Abs(width), Math.Abs(height));
        }
    }
}