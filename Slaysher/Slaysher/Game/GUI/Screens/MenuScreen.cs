using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Components;

namespace Slaysher.Game.GUI.Screens
{
    public class MenuScreen : GameScreen
    {
        private readonly List<GuiItem> _menuEntries = new List<GuiItem>();

        //Component Vars
        public float ItemOffset { get; set; }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<GuiItem> MenuEntries
        {
            get { return _menuEntries; }
        }

        public MenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            ItemOffset = 15f;
        }

        /// <summary>
        /// The same as Update() except only active screens get called.
        /// </summary>
        /// <param name="gameTime">Current GameTime</param>
        /// <param name="input">Input Object to access Mouse and Keyboard</param>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            foreach (GuiItem menuEntry in MenuEntries)
            {
                menuEntry.HandleInput(input);
            }
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
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            foreach (GuiItem item in MenuEntries)
            {
                item.Update(this, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateMenuEntryLocations();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (GuiItem item in MenuEntries)
            {
                item.Draw(this, gameTime);
            }

            spriteBatch.End();
        }

        protected virtual void UpdateMenuEntryLocations()
        {
            float transitionOffset = (float) Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, 175f);

            int offsetCounter = 0;
            foreach (GuiItem item in MenuEntries)
            {
                position.X = ScreenManager.GraphicsDevice.Viewport.Width/2 - item.GetWidth(this)/2;
                position.Y += item.GetHeight(this) + (offsetCounter * ItemOffset);

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset*256;
                else
                    position.X += transitionOffset*512;

                item.Position = position;

                offsetCounter++;
            }
        }
    }
}