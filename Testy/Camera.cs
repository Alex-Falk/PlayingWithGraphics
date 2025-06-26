using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Testy
{
    internal class Camera
    {
        private const float c_defaultCameraSpeed = 2.0f;
        private static readonly float c_defaultCameraRotationSpeed = 2.0f;

        public Vector3 Position { get; set; } = Vector3.UnitZ * 3;

        public float Pitch
        {
            get => m_pitch;
            set
            {
                var angle = MathHelper.Clamp(value, -MathF.PI, MathF.PI);
                m_yaw = angle;
                UpdateVectors();
            }

        }

        public float Yaw
        {
            get => m_yaw;
            set
            {
                if (value < 0)
                {
                    value += ExtraMathF.TwoPI;
                }
                else if (value > ExtraMathF.TwoPI)
                {
                    value = ExtraMathF.TwoPI - value;
                }
                m_yaw = value;
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
        
        // TODO: should come from an options system
        private float m_sensitivity = 2;

        public Camera()
        {
            UpdateVectors();
        }
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
            return Matrix4.LookAt(Position, Position + Forward, Up);
        }

        public void OnUpdate(float dt, KeyboardState? keyboardState, MouseState? mouseState)
        {
            UpdateKeyboard(dt, keyboardState);
            UpdateMouse(dt, mouseState);
        }

        private void UpdateMouse(float dt, MouseState? mouseState)
        {
            if (mouseState == null)
            {
                return;
            }
        }

        private void UpdateKeyboard(float dt, KeyboardState? keyboardState)
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
                Yaw = m_yaw - (c_defaultCameraRotationSpeed * dt);
            }

            if (keyboardState.IsKeyDown(Keys.E))
            {
                Yaw = m_yaw + (c_defaultCameraRotationSpeed * dt);
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                Position += m_up * dt * c_defaultCameraSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                Position -= m_up * dt * c_defaultCameraSpeed;
            }
        }
    }
}
