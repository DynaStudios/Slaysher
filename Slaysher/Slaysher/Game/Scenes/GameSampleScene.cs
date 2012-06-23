namespace Slaysher.Game.Scenes
{
    public class GameSampleScene : IScene
    {
        public string Name
        {
            get { return "gameScene"; }
        }
        public Engine Engine { get; set; }

        public GameSampleScene(Engine engine)
        {
        }

        public void LoadScene()
        {
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
        }

        public void UnloadScene()
        {
        }
    }
}