using System;
using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class MainMenuScene : MenuScreen
    {
        public MainMenuScene()
        {
            Button playButton = new Button("Start Game");
            Button optionsButton = new Button("Options");
            Button exitButton = new Button("Exit Game");

            playButton.Clicked += StartGameButtonClicked;
            optionsButton.Clicked += OptionsButtonClicked;
            exitButton.Clicked += ExitGameButtonClicked;

            MenuEntries.Add(playButton);
            MenuEntries.Add(optionsButton);
            MenuEntries.Add(exitButton);
            
        }

        #region Overrides of GameScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            PresentationOffset = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 4) * 3.5f, (ScreenManager.GraphicsDevice.Viewport.Height / 4) * 2.25f);
        }

        #endregion

        private void StartGameButtonClicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameScene());
        }

        private void OptionsButtonClicked(object sender, EventArgs e)
        {
            //todo: To be implemented with options branch
        }

        private void ExitGameButtonClicked(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}