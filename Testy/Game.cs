using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GL = OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Testy
{
    internal class Game : GameWindow
    {
        private List<RenderableObject> Objects { get; set; } = new List<RenderableObject>() 
            { 
                new RenderableObject("Meshes/Cube.obj", 3 * Vector3.UnitX, Quaternion.Identity, Color4.Red) , 
                new RenderableObject("Meshes/capy.obj", 3 * Vector3.UnitZ, Quaternion.FromEulerAngles(MathHelper.PiOver2, 0, 0), Color4.Green) ,
                new RenderableObject("Meshes/Cube.obj", 3 * Vector3.UnitY, Quaternion.Identity, Color4.Blue)
            };

        private Matrix4 m_projectionMatrix;
        private Matrix4 m_modelMatrix;
        private Matrix4 m_viewMatrix;
        private Matrix4 m_textureMatrix;
        private Shader m_shader;
        private Camera m_camera;
        
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var input = KeyboardState;
            if (input != null)
            {
                if (input.IsKeyDown(Keys.Escape))
                {
                    Close();
                }
            }

            foreach (var objects in Objects)
            {
                objects.OnUpdateFrame(args);
            }

            m_camera.OnUpdate((float)args.Time, input);

            m_viewMatrix = m_camera.GenerateViewMatrix();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            m_shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            m_shader.Use();

            foreach (var objects in Objects)
            {
                objects.OnLoad();
            }

            m_camera = new Camera();
        }

        protected override void OnUnload()
        {
            foreach (var objects in Objects)
            {
                objects.OnUnload();
            }

            GL.DeleteProgram(m_shader.Handle);
            m_shader.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            //base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            m_shader.SetUniform("uProjectionMtx", m_projectionMatrix);
            m_shader.SetUniform("uViewMtx", m_viewMatrix);
            foreach (var objects in Objects)
            {
                objects.OnRenderFrame(ref m_shader);
            }
            
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            m_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)e.Width / (float)e.Height, 0.01f, 100.0f);
        }
    }
}
