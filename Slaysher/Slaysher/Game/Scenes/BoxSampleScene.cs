using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Graphics.Camera;

namespace Slaysher.Game.Scenes
{
    public class BoxSampleScene : IScene
    {
        public Engine Engine { get; set; }

        private Camera _camera = new Camera();

        private Matrix _cubeWorld;
        private Model _cubeModel;

        public BoxSampleScene(Engine engine)
        {
            Engine = engine;
        }

        public void LoadScene()
        {
            _cubeModel = Engine.Content.Load<Model>("Cube");

            _cubeWorld = Matrix.Identity;
        }

        public void Render(GameTime time)
        {
            drawModel(_cubeModel, _cubeWorld);
        }

        public void Update(GameTime time)
        {
            KeyboardState keyBoardState = Keyboard.GetState();

            //Rotate Cube along its Up Vector
            if (keyBoardState.IsKeyDown(Keys.X))
            {
                _cubeWorld = Matrix.CreateFromAxisAngle(Vector3.Up, .02f) * _cubeWorld;
            }
            if (keyBoardState.IsKeyDown(Keys.Z))
            {
                _cubeWorld = Matrix.CreateFromAxisAngle(Vector3.Up, -.02f) * _cubeWorld;
            }

            //Move Cube Forward, Back, Left, and Right
            if (keyBoardState.IsKeyDown(Keys.Up))
            {
                _cubeWorld *= Matrix.CreateTranslation(_cubeWorld.Forward);
            }
            if (keyBoardState.IsKeyDown(Keys.Down))
            {
                _cubeWorld *= Matrix.CreateTranslation(_cubeWorld.Backward);
            }
            if (keyBoardState.IsKeyDown(Keys.Left))
            {
                _cubeWorld *= Matrix.CreateTranslation(-_cubeWorld.Right);
            }
            if (keyBoardState.IsKeyDown(Keys.Right))
            {
                _cubeWorld *= Matrix.CreateTranslation(_cubeWorld.Right);
            }

            _camera.Update();
        }

        public void UnloadScene()
        {
        }

        private void drawModel(Model model, Matrix worldMatrix)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = _camera.viewMatrix;
                    effect.Projection = _camera.projectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}