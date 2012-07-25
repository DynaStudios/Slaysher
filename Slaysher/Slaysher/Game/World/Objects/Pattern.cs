using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Graphics.Camera;

namespace Slaysher.Game.World.Objects
{
    public class Pattern
    {
        private readonly Texture2D _patternTexture;
        private readonly Vector3 _position;

        public Pattern(Vector3 position, Texture2D texture)
        {
            _patternTexture = texture;
            _position = position;
        }

        public void Draw(Model model, Matrix worldMatrix, Camera camera)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    //effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix * _position;
                    effect.TextureEnabled = true;
                    effect.Texture = _patternTexture;

                    effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    //effect.CurrentTechnique.Passes[0].Apply();

                    effect.World = modelTransforms[mesh.ParentBone.Index]*Matrix.CreateTranslation(_position);
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}