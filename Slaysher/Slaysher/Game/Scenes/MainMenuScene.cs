using System;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class MainMenuScene : MenuScreen
    {
         public MainMenuScene()
         {
             Button playButton = new Button("Start Game");
             Button exitButton = new Button("Exit Game");

             playButton.Clicked += StartGameButtonClicked;
             exitButton.Clicked += ExitGameButtonClicked;

             MenuEntries.Add(playButton);
             MenuEntries.Add(exitButton);
         }

         private void StartGameButtonClicked(object sender, EventArgs e)
         {
             LoadingScreen.Load(ScreenManager, true, new GameScene());
         }

        private void ExitGameButtonClicked(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

    }
}