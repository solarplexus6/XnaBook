using Microsoft.Xna.Framework;

namespace CubeChaser
{
    internal class Camera
    {
        #region Private fields

        private Vector3 baseCameraReference = new Vector3(0, 0, 1);
        private Vector3 lookAt;
        private bool needViewResync = true;
        private Vector3 position = Vector3.Zero;
        private float rotation;

        #endregion
        #region Properties

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateLookAt();
            }
        }

        public Matrix Projection { get; private set; }

        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                UpdateLookAt();
            }
        }

        #endregion
        #region Ctors

        public Camera(
            Vector3 position,
            float rotation,
            float aspectRatio,
            float nearClip,
            float farClip)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                aspectRatio,
                nearClip,
                farClip);
            MoveTo(position, rotation);
        }

        #endregion
        #region Public methods

        public void MoveTo(Vector3 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
            UpdateLookAt();
        }

        #endregion
        #region Private methods

        private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation);
            Vector3 lookAtOffset = Vector3.Transform(baseCameraReference, rotationMatrix);
            lookAt = position + lookAtOffset;
            needViewResync = true;
        }

        private Matrix cachedViewMatrix;

        public Matrix View
        {
            get
            {
                if (needViewResync)
                    cachedViewMatrix = Matrix.CreateLookAt(
                    Position,
                    lookAt,
                    Vector3.Up);
                return cachedViewMatrix;
            }
        }

        public Vector3 PreviewMove(float scale)
        {
            Matrix rotate = Matrix.CreateRotationY(rotation);
            Vector3 forward = new Vector3(0, 0, scale);
            forward = Vector3.Transform(forward, rotate);
            return (position + forward);
        }
        public void MoveForward(float scale)
        {
            MoveTo(PreviewMove(scale), rotation);
        }

        #endregion
    }
}