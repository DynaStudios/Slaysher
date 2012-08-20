using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Screens;
using Slaysher.Game.IO;

namespace Slaysher.Game.GUI
{
    public class ScreenManager : DrawableGameComponent
    {
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly List<GameScreen> _tempScreenList = new List<GameScreen>();

        private readonly InputState _input;

        public SpriteBatch SpriteBatch { get; protected set; }
        public SpriteFont Font { get; protected set; }
        public SpriteFont SmallFont { get; protected set; }
        public Texture2D BlankTexture { get; protected set; }

        public bool TraceEnabled { get; set; }

        private bool _isInitialized;
        
        public ScreenManager(Engine game) : base(game)
        {
            TraceEnabled = false;
            _input = new InputState();
        }

        #region Overrides of DrawableGameComponent

        public override void Initialize()
        {
            base.Initialize();

            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Font = content.Load<SpriteFont>("Fonts/menu");
            SmallFont = content.Load<SpriteFont>("Fonts/menu_small");
            BlankTexture = content.Load<Texture2D>("Images/Game/dyna_splash");

            //Let all screens load their content
            foreach (GameScreen screen in _screens)
            {
                screen.Activate(false);
            }
        }

        protected override void UnloadContent()
        {
            //Let all screens unload their content
            foreach (GameScreen screen in _screens)
            {
                screen.Unload();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Let all screens draw their content
            var zSortedList = (from a in _screens orderby a.ZIndex ascending select a).ToArray();
            foreach (GameScreen screen in zSortedList)
            {
                if(screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        #endregion

        #region Overrides of GameComponent

        public override void Update(GameTime gameTime)
        {
            //Update Mouse and Keyboard Handlers
            _input.Update(gameTime);

            _tempScreenList.Clear();

            foreach (GameScreen gameScreen in _screens)
            {
                _tempScreenList.Add(gameScreen);
            }

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            //Loop as long screens waiting for update
            while (_tempScreenList.Count > 0)
            {
                var screen = _tempScreenList[_tempScreenList.Count - 1];
                _tempScreenList.RemoveAt(_tempScreenList.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    bool otherScreenIsPopup = _screens.Any(gameScreen => gameScreen.IsPopup);

                    if (!otherScreenHasFocus || otherScreenIsPopup)
                    {
                        screen.HandleInput(gameTime, _input);
                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                    {
                        coveredByOtherScreen = true;
                    }
                }

                if (TraceEnabled)
                {
                    TraceScreens();
                }
            }

        }

        #endregion

        private void TraceScreens()
        {
            Debug.WriteLine(string.Join(", ", _screens.Select(screen => screen.GetType().Name).ToArray()));
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (_isInitialized)
            {
                screen.Activate(false);
            }

            _screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (_isInitialized)
            {
                screen.Unload();
            }

            _screens.Remove(screen);
            _tempScreenList.Remove(screen);
        }

        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        public void FadeBackBufferToBlack(float alpha)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BlankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            SpriteBatch.End();
        }
    }
}