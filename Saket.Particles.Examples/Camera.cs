using OpenTK.Mathematics;
using System;


    public class Camera
    {
        public Camera(Vector3 position,float fov, float aspectRatio)
        {
            this.position = position;
            this._fov = MathHelper.DegreesToRadians(fov);
            AspectRatio = aspectRatio;

            CalcuateViewMatrix();
            CalculateProjectionMatrix();
        }

        /// <summary>
        /// Position of the camera
        /// </summary>
        public Vector3 Position => position;
        public Matrix4 ProjectionMatrix => projectionMatrix;
        public Matrix4 ViewMatrix => viewMatrix;



        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;




        // This is simply the aspect ratio of the viewport, used for the projection matrix.
        public float AspectRatio { get; set; }

        private Vector3 position;
        // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;


        private Matrix4 viewMatrix;
        private Matrix4 projectionMatrix;



        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;


        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        // This has been discussed more in depth in a previous tutorial,
        // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
                CalculateProjectionMatrix();
            }
        }



        public void Translate(Vector3 translation)
        {
            position += translation;
            CalcuateViewMatrix();
        }

 
        private void CalcuateViewMatrix()
        {
            viewMatrix = Matrix4.LookAt(Position, Position + _front, _up);
        }
        private void CalculateProjectionMatrix()
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            _front = Vector3.Normalize(_front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
            CalcuateViewMatrix();
        }
    }
