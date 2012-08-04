using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Slaysher.Graphics;
using SlaysherNetworking.Game.World;

namespace Slaysher.Graphics.Camera
{
    public class Camera
    {
        private WorldPosition _target;
        private Vector3 _position;
        //private Vector3 _targetPosition;
        private Matrix _cameraRotation;

        //private Vector3 _desiredPosition;
        //private Vector3 _desiredTarget;
        private Vector3 _offsetDistance;

        public Matrix ViewMatrix, ProjectionMatrix;

        public float Yaw, Pitch, Roll;
        public float Speed;
        public WorldPosition Target
        {
            set { _target = value; }
            get { return _target; }
        }

        public Camera(WorldPosition target)
        {
            _target = target;
            ResetCamera();
        }

        public void ResetCamera()
        {
            _offsetDistance = new Vector3(0, 15, 5);
            _position = _target.CreateOnSurfacePosition() + _offsetDistance;
            // new Vector3(0, 0, 50);
            //_target = new Vector3();

            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 16/9, .5f, 500f);

            Yaw = 0.0f;
            Pitch = 0.0f;
            Roll = 0.0f;

            Speed = .3f;

            _cameraRotation = Matrix.Identity;
        }

        public void Update(Matrix chasedObjectsWorld)
        {
            if (_target != null)
            {
                UpdateViewMatrix(chasedObjectsWorld);
            }
        }

        private void UpdateViewMatrix(Matrix chasedObjectsWorld)
        {

            _cameraRotation.Forward.Normalize();
            chasedObjectsWorld.Right.Normalize();
            chasedObjectsWorld.Up.Normalize();

            Vector3 targetPosition = Target.CreateOnSurfacePosition();

            _cameraRotation = Matrix.CreateFromAxisAngle(_cameraRotation.Forward, Roll);
            Vector3 desiredPosition = Vector3.Transform(_offsetDistance + targetPosition, chasedObjectsWorld);
            _position = Vector3.SmoothStep(_position, desiredPosition, .15f);

            Yaw = MathHelper.SmoothStep(Yaw, 0f, .1f);
            Pitch = MathHelper.SmoothStep(Pitch, 0f, .1f);
            Roll = MathHelper.SmoothStep(Roll, 0f, .2f);
            ViewMatrix = Matrix.CreateLookAt(_position, _target.CreateOnSurfacePosition(), Vector3.Up);
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