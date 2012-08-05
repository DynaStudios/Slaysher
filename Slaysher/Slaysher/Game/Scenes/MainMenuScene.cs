using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class MainMenuScene : MenuScreen
    {
        public MainMenuScene()
        {
        }

        #region Overrides of GameScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            PresentationOffset = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 4) * 3.5f, (ScreenManager.GraphicsDevice.Viewport.Height / 4) * 2.25f);

            SoundEffect hoverSound = ScreenManager.Game.Content.Load<SoundEffect>("Sounds/GUI/btnHover");
            SoundEffect clickSound = ScreenManager.Game.Content.Load<SoundEffect>("Sounds/GUI/click");

            Button playButton = new Button("Start Game") { HoverSound = hoverSound, ClickSound = clickSound};
            Button panelButton = new Button("Panel Test") { HoverSound = hoverSound, ClickSound = clickSound };
            Button exitButton = new Button("Exit Game") { HoverSound = hoverSound, ClickSound = clickSound };

            playButton.Clicked += StartGameButtonClicked;
            panelButton.Clicked += PanelButtonClicked;
            exitButton.Clicked += ExitGameButtonClicked;

            MenuEntries.Add(playButton);
            MenuEntries.Add(panelButton);
            MenuEntries.Add(exitButton);
        }

        #endregion

        private void StartGameButtonClicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameScene());
        }

        private void PanelButtonClicked(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new TestPanelScreen {Title = "Panel Test"});
        }

        private void ExitGameButtonClicked(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}