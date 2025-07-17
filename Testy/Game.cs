using System.Diagnostics;
using Imgui;
using ImGuiNET;
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
        ImGuiController m_imguiController;
        
        private List<ObjectBase> Objects { get; set; } = new List<ObjectBase>() 
            { 
                new RenderableObject(OBJLibrary.Instance.Meshes["cube"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(0, 0, 0), Quaternion.Identity, Color4.Blue),
                new RenderableObject(OBJLibrary.Instance.Meshes["Sphere"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(5, 0, 0), Quaternion.Identity, Color4.Red),
                new RenderableObject(OBJLibrary.Instance.Meshes["cube"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(-5, 0, 0), Quaternion.Identity, Color4.Blue),
                new RenderableObject(OBJLibrary.Instance.Meshes["Sphere"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(0, 5, 0), Quaternion.Identity, Color4.Red),
                new RenderableObject(OBJLibrary.Instance.Meshes["cube"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(0, -5, 0), Quaternion.Identity, Color4.Blue),
                new RenderableObject(OBJLibrary.Instance.Meshes["Sphere"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(0, 0, 5), Quaternion.Identity, Color4.Red),
                new RenderableObject(OBJLibrary.Instance.Meshes["Sphere"], MaterialLibrary.Instance.Materials["Gold"], "Textures/test_from_tutorial.png", new Vector3(0, 0, -5), Quaternion.Identity, Color4.Red),
            };

        private Matrix4 m_projectionMatrix;
        private Matrix4 m_viewMatrix;
        private Shader m_shader;
        private Camera m_camera;
        private float m_time;

        public Game(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            LightSource.SetupLightSource(Vector3.One, Color4.White);
            
            m_imguiController = new ImGuiController(width, height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // TODO Make an input system
            var keyboardInput = KeyboardState;
            if (keyboardInput != null)
            {
                if (keyboardInput.IsKeyDown(Keys.Escape))
                {
                    Close();
                }
            }
            
            foreach (var objects in Objects)
            {
                objects.OnUpdateFrame(args);
            }

            m_camera.OnUpdate((float)args.Time, keyboardInput, MouseState);

            m_viewMatrix = m_camera.GenerateViewMatrix();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            m_shader = new Shader("Shaders/shader.vert", "Shaders/phongLighting.frag");
            m_shader.Use();

            GL.Enable(EnableCap.DepthTest);
            
            foreach (var objects in Objects)
            {
                objects.OnLoad();
            }

            m_camera = new Camera();
            
            //CursorState = CursorState.Grabbed;
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
            m_imguiController.Update(this, (float)args.Time);
            
            m_time += (float)args.Time;
            if (m_time >= ExtraMath.TwoPI)
            {
                m_time = 0;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            m_shader.SetUniform("uProjectionMtx", m_projectionMatrix);
            m_shader.SetUniform("uViewMtx", m_viewMatrix);
            m_shader.TrySetUniform("viewPos", m_camera.Position);
            
            LightSource.Instance.OnRenderFrame(ref m_shader); // Needs to happen before any of the objects...
            foreach (var objects in Objects)
            {
                objects.OnRenderFrame(ref m_shader);
            }
            
            // Enable Docking
            // ImGui.DockSpaceOverViewport();
            //
            //ImGui.ShowDemoWindow();

            DebugUpdate(args.Time);
            
            m_imguiController.Render();
            //
            // ImGuiController.CheckGLError("End of frame");
            
            SwapBuffers();
        }

        private void DebugUpdate(double argsTime)
        {
            ImGui.SetKeyboardFocusHere();
            
            ImGui.Begin("Game Debug");
            {
                ImGui.Text($"FPS: {1.0 / argsTime:0.00}");
                var lightPos = LightSource.Instance.Position.ToSystemVec3();
                ImGui.Text($"Camera Position: {m_camera.Position}");
                ImGui.Text($"Projection Matrix: {m_projectionMatrix}");
                ImGui.Text($"View Matrix: {m_viewMatrix}");

                ImGui.SliderFloat3("Light Position", ref lightPos, - 100.0f, 100.0f);
                LightSource.Instance.Position = lightPos.ToOpenTkVec3();

            }
            ImGui.End();
        }
 
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            m_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)e.Width / (float)e.Height, 0.01f, 100.0f);
            
            m_imguiController.WindowResized(ClientSize.X, ClientSize.Y);
        }
    }
}
