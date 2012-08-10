using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Slaysher.Game;
using Slaysher.Graphics;
using Slaysher.Graphics.Camera;
using Slaysher.Network;

using SlaysherNetworking.Game.World;
using SlaysherNetworking.Game.Entities;
using SlaysherNetworking.Packets;

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

    public class ClientPlayer : ClientEntity, IPlayer
    {
        public string Nickname { get; set; }
        private Client _client;
        private WorldPosition _position;
        public WorldPosition Position
        {
            get { return _position; }
            set
            {
                if (_position == null)
                {
                    _position = new WorldPosition();
                }
                _position.X = value.X;
                _position.Y = value.Y;
                if (VisualPosition == null)
                {
                    VisualPosition = new WorldPosition(_position);
                }
            }
        }
        public Model Model { get; set; }
        public WorldPosition VisualPosition { get; set; }
        private float? _lastMovementDirection = null;

        public ClientPlayer(Client client)
        {
            _client = client;
        }

        public void Update(Matrix chasedObjectsWorld)
        {
        }

        private void HandleInput()
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
                SendMove((float)direction);
            }
            else
            {
                SendStopMoving();
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
                    effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    effect.World = (modelTransforms[mesh.ParentBone.Index] * playerModelScale) * position3dMatrix;
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

        public override void Tick(TimeSpan totalTime)
        {
            this.ExecutePreparedMove(totalTime);
            this.ExecuteMovement(totalTime);

            smoothMove();
            HandleInput();
        }

        private void SendMove(float direction)
        {
            if (_lastMovementDirection != null && _lastMovementDirection == direction)
            {
                return;
            }
            _lastMovementDirection = direction;
            MovePacket movePacket = new MovePacket
            {
                EntityId = Id,
                Direction = direction,
                Speed = Speed
            };
            _client.SendPacket(movePacket);
        }

        private void SendStopMoving()
        {
            if (_lastMovementDirection == null)
            {
                return;
            }
            _lastMovementDirection = null;
            MovePacket movePacket = new MovePacket
            {
                EntityId = Id,
                Direction = 0,
                Speed = 0
            };
            _client.SendPacket(movePacket);
        }
    }
}
