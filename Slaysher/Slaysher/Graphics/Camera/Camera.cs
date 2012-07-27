using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Slaysher.Graphics.Camera
{
    public class Camera
    {
        private Vector3 _position;
        private Vector3 _target;
        public Matrix ViewMatrix, ProjectionMatrix;

        public float Yaw, Pitch, Roll;
        public float Speed;
        private Matrix _cameraRotation;

        private Vector3 _desiredPosition;
        private Vector3 _desiredTarget;
        private Vector3 _offsetDistance;

        public Camera()
        {
            ResetCamera();
        }

        public void ResetCamera()
        {
            _position = new Vector3(0, 0, 50);
            _target = new Vector3();

            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 16/9, .5f, 500f);

            _desiredTarget = _target;
            _desiredPosition = _position;

            _offsetDistance = new Vector3(0, 15, 20);

            Yaw = 0.0f;
            Pitch = 0.0f;
            Roll = 0.0f;

            Speed = .3f;

            _cameraRotation = Matrix.Identity;
        }

        public void Update(Matrix chasedObjectsWorld)
        {
            HandleInput();
            UpdateViewMatrix(chasedObjectsWorld);
        }

        private void UpdateViewMatrix(Matrix chasedObjectsWorld)
        {
            ViewMatrix = Matrix.CreateLookAt(_position, _target, Vector3.Up);

            _cameraRotation.Forward.Normalize();
            chasedObjectsWorld.Right.Normalize();
            chasedObjectsWorld.Up.Normalize();

            _cameraRotation = Matrix.CreateFromAxisAngle(_cameraRotation.Forward, Roll);

            _desiredTarget = chasedObjectsWorld.Translation;
            _target = _desiredTarget;
            _target.X += Yaw;
            _target.Y += Pitch;

            _desiredPosition = Vector3.Transform(_offsetDistance, chasedObjectsWorld);
            _position = Vector3.SmoothStep(_position, _desiredPosition, .15f);

            Yaw = MathHelper.SmoothStep(Yaw, 0f, .1f);
            Pitch = MathHelper.SmoothStep(Pitch, 0f, .1f);
            Roll = MathHelper.SmoothStep(Roll, 0f, .2f);
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.J))
            {
                Yaw += .02f;
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                Yaw += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.I))
            {
                Pitch += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                Pitch += .02f;
            }
            if (keyboardState.IsKeyDown(Keys.U))
            {
                Roll += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.O))
            {
                Roll += .02f;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                MoveCamera(_cameraRotation.Forward);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                MoveCamera(-_cameraRotation.Forward);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                MoveCamera(-_cameraRotation.Right);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                MoveCamera(_cameraRotation.Right);
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                MoveCamera(_cameraRotation.Up);
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                MoveCamera(-_cameraRotation.Up);
            }
        }

        private void MoveCamera(Vector3 addedVector)
        {
            _position += Speed*addedVector;
        }
    }
}