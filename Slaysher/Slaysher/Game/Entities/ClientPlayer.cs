using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Slaysher.Game;
using Slaysher.Graphics;
using Slaysher.Graphics.Camera;
using SlaysherNetworking.Game.World;
using SlaysherNetworking.Game.Entities;

namespace Slaysher.Game.Entities
{
    class Directions
    {
        public const float Up = 0;
        public const float UpRight = 45.0f;
        public const float Right = 90.0f;
        public const float DownRight = 135.0f;
        public const float Down = 180.0f;
        public const float DownLeft = 225.0f;
        public const float Left = 270.0f;
        public const float UpLeft = 315.0f;
    }

    public class ClientPlayer : Player
    {
        public override WorldPosition Position
        {
            get { return base.Position; }
            set
            {
                if (base.Position == null)
                {
                    base.Position = new WorldPosition();
                }
                base.Position.X = value.X;
                base.Position.Y = value.Y;
                if (VisualPosition == null)
                {
                    VisualPosition = new WorldPosition(base.Position);
                }
            }
        }
        public Model Model { get; set; }
        public WorldPosition VisualPosition { get; set; }

        public void Update(Matrix chasedObjectsWorld)
        {
        }

        private void HandleInput(GameTime time)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float? direction = null;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (!keyboardState.IsKeyDown(Keys.S))
                {
                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        direction = Directions.UpLeft;
                    }
                    else if (keyboardState.IsKeyDown(Keys.D))
                    {
                        direction = Directions.UpRight;
                    }
                    else
                    {
                        direction = Directions.Up;
                    }
                }
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    direction = Directions.DownLeft;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    direction = Directions.DownRight;
                }
                else
                {
                    direction = Directions.Down;
                }
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    direction = Directions.Left;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    direction = Directions.Right;
                }
            }

            if (direction != null)
            {
                Move(time.TotalGameTime, (float)direction);
            }
            else
            {
                StopMoving(time.TotalGameTime);
            }
        }

        private void smoothMove()
        {
            //VisualPosition.MoveASmoothStepTo(Position, 0.025f);
            VisualPosition.X = MathHelper.SmoothStep(VisualPosition.X, Position.X, 0.35f);
            VisualPosition.Y = MathHelper.SmoothStep(VisualPosition.Y, Position.Y, 0.35f);
        }

        public void Render(Camera camera)
        {
            Matrix[] modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            Matrix playerModelScale = Matrix.CreateScale(ModelScaling);
            Matrix playerPositionScale = Matrix.CreateScale(1.0f / ModelScaling);
            Vector3 position3d = new Vector3(VisualPosition.X, 2.0f, VisualPosition.Y);
            Matrix position3dMatrix = Matrix.CreateTranslation(position3d);

            foreach (ModelMesh mesh in Model.Meshes)
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

                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }

        public void LoadPlayerModel(ContentManager content)
        {
            Model = content.Load<Model>("Models/Pattern/Player/goblin_fbx");
        }

        public void Tick(GameTime time)
        {
            HandleInput(time);
            smoothMove();
            //TODO: Update stuff like position etc. here
        }
    }
}
