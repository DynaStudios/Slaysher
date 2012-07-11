using Microsoft.Xna.Framework;

namespace Slaysher.Game.Scenes
{
    public interface IScene
    {
        // prefered interface attribute over reflection property because it is enforcable
        string Name { get; }

        void LoadScene();

        void Render(GameTime time);

        void Update(GameTime time);

        void UnloadScene();
    }
}