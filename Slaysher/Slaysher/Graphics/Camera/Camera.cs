using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slaysher.Graphics.Camera
{
    public class Camera
    {
        private Vector3 position;
        private Vector3 target;
        public Matrix viewMatrix, projectionMatrix;

        public float yaw, pitch, roll;
        public float speed;
        private Matrix cameraRotation;

        private Vector3 desiredPosition;
        private Vector3 desiredTarget;
        private Vector3 offsetDistance;

        public Camera()
        {
            ResetCamera();
        }

        public void ResetCamera()
        {
            position = new Vector3(0, 0, 50);
            target = new Vector3();

            viewMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 16 / 9, .5f, 500f);

            desiredTarget = target;
            desiredPosition = position;

            offsetDistance = new Vector3(0, 50, 175);

            yaw = 0.0f;
            pitch = 0.0f;
            roll = 0.0f;

            speed = .3f;

            cameraRotation = Matrix.Identity;
        }

        public void Update(Matrix chasedObjectsWorld)
        {
            HandleInput();
            UpdateViewMatrix(chasedObjectsWorld);
        }

        private void UpdateViewMatrix(Matrix chasedObjectsWorld)
        {
            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

            cameraRotation.Forward.Normalize();
            chasedObjectsWorld.Right.Normalize();
            chasedObjectsWorld.Up.Normalize();

            cameraRotation = Matrix.CreateFromAxisAngle(cameraRotation.Forward, roll);

            desiredTarget = chasedObjectsWorld.Translation;
            target = desiredTarget;
            target.X += yaw;
            target.Y += pitch;

            desiredPosition = Vector3.Transform(offsetDistance, chasedObjectsWorld);
            position = Vector3.SmoothStep(position, desiredPosition, .15f);

            yaw = MathHelper.SmoothStep(yaw, 0f, .1f);
            pitch = MathHelper.SmoothStep(pitch, 0f, .1f);
            roll = MathHelper.SmoothStep(roll, 0f, .2f);
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.J))
            {
                yaw += .02f;
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                yaw += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.I))
            {
                pitch += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                pitch += .02f;
            }
            if (keyboardState.IsKeyDown(Keys.U))
            {
                roll += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.O))
            {
                roll += .02f;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                MoveCamera(cameraRotation.Forward);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                MoveCamera(-cameraRotation.Forward);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                MoveCamera(-cameraRotation.Right);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                MoveCamera(cameraRotation.Right);
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                MoveCamera(cameraRotation.Up);
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                MoveCamera(-cameraRotation.Up);
            }
        }

        private void MoveCamera(Vector3 addedVector)
        {
            position += speed * addedVector;
        }
    }
}