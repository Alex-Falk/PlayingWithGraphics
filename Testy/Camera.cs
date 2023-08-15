using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Testy
{
    internal class Camera
    {
        private const float c_defaultCameraSpeed = 2.0f;

        public Vector3 Position { get; set; } = Vector3.UnitZ * 3;

        public float Pitch
        {
            get => m_pitch;
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                m_yaw = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }

        }

        public float Yaw
        {
            get => m_yaw;
            set
            {
                m_yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public Vector3 Forward => m_forward;
        public Vector3 Right => m_right;
        public Vector3 Up => m_up;

        private float m_pitch = 0.0f;
        private float m_yaw = -MathHelper.PiOver2;
        
        private Vector3 m_forward;
        private Vector3 m_up;
        private Vector3 m_right;
        
        private void UpdateVectors()
        {
            m_forward.X = MathF.Cos(Pitch) * MathF.Cos(Yaw);
            m_forward.Y = MathF.Sin(Pitch);
            m_forward.Z = MathF.Cos(Pitch) * MathF.Sin(Yaw);
            m_forward.Normalize();

            m_right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
            m_up = Vector3.Normalize(Vector3.Cross(Right, Forward));
        }

        public Matrix4 GenerateViewMatrix()
        {
            return Matrix4.LookAt(Position, Forward, Up);
        }

        public void OnUpdate(float dt, KeyboardState? keyboardState)
        {
            if (keyboardState == null)
                return;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                Position += m_forward * dt * c_defaultCameraSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Position -= m_forward * dt * c_defaultCameraSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                Position -= m_right * dt * c_defaultCameraSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Position += m_right * dt * c_defaultCameraSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Q))
            {
                Yaw += c_defaultCameraSpeed * dt;
            }

            if (keyboardState.IsKeyDown(Keys.E))
            {
                Yaw -= c_defaultCameraSpeed * dt;
            }

        }
    }
}
