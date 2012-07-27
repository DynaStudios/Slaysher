using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SlaysherNetworking.Game.Entities;

namespace Slaysher.Game.Scenes
{
    public partial class GameScene
    {
        public Player Player { get; set; }
        private Model _playerModel;

        private void TickPlayer(GameTime time)
        {
            Player.ExecuteMovement(time.ElapsedGameTime);
            //TODO: Update stuff like position etc. here
        }

        private void loadPlayerModel()
        {
            _playerModel = Engine.Content.Load<Model>("Models/Pattern/Player/goblin_fbx");
        }

        private void renderPlayer()
        {
            Matrix[] modelTransforms = new Matrix[_playerModel.Bones.Count];
            _playerModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in _playerModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    //effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix * _position;
                    //effect.TextureEnabled = true;
                    //effect.Texture = _patternTexture;

                    effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    //effect.CurrentTechnique.Passes[0].Apply();

                    Vector3 position3d = new Vector3(Player.Position.X, 5, Player.Position.Y);

                    effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(position3d);
                    effect.View = _tempCamera.viewMatrix;
                    effect.Projection = _tempCamera.projectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}