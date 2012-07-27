using System;
using Microsoft.Xna.Framework;

namespace Slaysher.Game.GUI.Screens
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden
    }

    public abstract class GameScreen
    {
        /// <summary>
        /// Normally when a screen is on top of another screen, the screen will be automaticly transitioned. 
        /// If IsPopup is true, the screen won't be transitioned away.
        /// </summary>
        public bool IsPopup { get; protected set; }

        /// <summary>
        /// Time to transition to the screen.
        /// </summary>
        public TimeSpan TransitionOnTime { get; protected set; }

        /// <summary>
        /// Time to transition off the screen
        /// </summary>
        public TimeSpan TransitionOffTime { get; protected set; }

        /// <summary>
        /// Current Transition Position. Ranging from 0 to 1.
        /// 0 = Fully Visible. 1 = Not Visible
        /// </summary>
        public float TransitionPosition { get; protected set; }

        /// <summary>
        /// Current Alpha value for transition.
        /// 1 = Total Visible. 0 = Not Visible
        /// </summary>
        public float TransitionAlpha { get { return 1f - TransitionPosition; } }

        /// <summary>
        /// Current ScreenState
        /// </summary>
        public ScreenState ScreenState { get; protected set; }

        /// <summary>
        /// If set to true. Screen is going to be closed.
        /// </summary>
        public bool IsExiting { get; protected internal set; }

        /// <summary>
        /// Checks if Screen is visible and able to handle user input
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !_otherScreenHasFocus &&
                       (ScreenState == ScreenState.TransitionOn || ScreenState == ScreenState.Active);
            }
        }

        private bool _otherScreenHasFocus;

        /// <summary>
        /// Gets the manager this screen belongs to
        /// </summary>
        public ScreenManager ScreenManager { get; internal set; }

        protected GameScreen()
        {
            TransitionPosition = 1;
            ScreenState = ScreenState.TransitionOn;
        }

        public virtual void Activate(bool instancePreserved)
        {
            
        }

        public virtual void Deactivate()
        {
            
        }

        public virtual void Unload()
        {
            
        }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                // If the screen is going away to die, it should transition off.
                ScreenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                ScreenState = UpdateTransition(gameTime, TransitionOffTime, 1) ? ScreenState.TransitionOff : ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                ScreenState = UpdateTransition(gameTime, TransitionOnTime, -1) ? ScreenState.TransitionOn : ScreenState.Active;
            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (TransitionPosition <= 0)) ||
                ((direction > 0) && (TransitionPosition >= 1)))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        public virtual void HandleInput(GameTime gameTime, InputState input) { }

        public virtual void Draw(GameTime gameTime) { }

        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                IsExiting = true;
            }
        }
    }
}