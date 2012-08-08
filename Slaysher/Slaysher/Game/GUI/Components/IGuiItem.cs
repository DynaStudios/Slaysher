using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public interface IGuiItem
    {
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }
        int ZIndex { get; set; }
        
        //Sound Effects
        SoundEffect HoverSound { get; set; }
        SoundEffect ClickSound { get; set; }

        float GetWidth(GameScreen gameScreen);
        float GetHeight(GameScreen gameScreen);
        void Update(GameScreen gameScreen, GameTime gameTime);
        void Draw(GameScreen gameScreen, GameTime gameTime);
        void HandleInput(InputState input);
    }
}