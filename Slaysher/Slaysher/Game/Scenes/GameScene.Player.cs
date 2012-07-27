using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlaysherNetworking.Game.World;

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
            Player.Position = new WorldPosition(20.0f, 20.0f);
            Player.ModelScaling = 1f/128f;
        }

        private void renderPlayer()
        {
            Matrix[] modelTransforms = new Matrix[_playerModel.Bones.Count];
            _playerModel.CopyAbsoluteBoneTransformsTo(modelTransforms);
            Matrix playerModelScale = Matrix.CreateScale(Player.ModelScaling);
            Matrix playerPositionScale = Matrix.CreateScale(1.0f / Player.ModelScaling);
            Vector3 position3d = new Vector3(Player.Position.X, 2.0f, Player.Position.Y);
            Matrix position3dMatrix = Matrix.CreateTranslation(position3d);

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

                    effect.World = (modelTransforms[mesh.ParentBone.Index] * playerModelScale)
                            * position3dMatrix;

                    effect.View = _tempCamera.ViewMatrix;
                    effect.Projection = _tempCamera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}