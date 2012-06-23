using Microsoft.Xna.Framework;

namespace Slaysher.Game.Scenes
{
    public interface IScene
    {
        string Name { get; }

        void LoadScene();

        void Render(GameTime time);

        void Update(GameTime time);

        void UnloadScene();
    }
}