using System;
using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;
using Slaysher.Game.Scenes.WorldGen;

namespace Slaysher.Game.Scenes
{
    public class WorldGenTest : GameScreen
    {
        #region Overrides of GameScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            WorldGenMenu menu = new WorldGenMenu();
            
            Button generateButton = new Button("Generate");
            generateButton.Clicked += GenerateButtonClicked;

            menu.PanelEntries.Add(generateButton);

            ScreenManager.AddScreen(menu);

            //Load other content to generate stuff

            //Render first perlin noise before hiding loading screen...
        }

        private void GenerateButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Generate clicked!");

            //Create new Perlin noise texture here
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion
    }
}