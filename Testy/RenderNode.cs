using ObjLoader.Loader.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Testy
{
    internal class RenderableObject
    {
        public RenderableObject()
        {
            m_vetices.AddRange( new []{
                0.5f, 0.5f, 0.0f, // top right
                0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, // bottom left
                -0.5f, 0.5f, 0.0f, // top left
            });
            
            m_indeces.AddRange(new []
            {
                0, 1, 3, // The first triangle will be the top-right half of the triangle
                1, 2, 3  // Then the second will be the bottom-left half of the triangle
            });
        }

        public RenderableObject(string filePath)
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();

            var fileStream = new FileStream(filePath, FileMode.Open);
            var result = objLoader.Load(fileStream);

            fileStream.Close();

            foreach (var vertex in result.Vertices)
            {
                m_vetices.Add(vertex.X);
                m_vetices.Add(vertex.Y);
                m_vetices.Add(vertex.Z);
            }

            foreach (var normal in result.Normals)
            {
                m_normals.Add(normal.X);
                m_normals.Add(normal.Y);
                m_normals.Add(normal.Z);
            }

            foreach (var group in result.Groups)
            {
                foreach (var face in group.Faces)
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        m_indeces.Add(face[i].VertexIndex);
                    }
                }
            }
        }

        public void OnLoad()
        {
            m_vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vetices.Count * sizeof(float), m_vetices.ToArray(), BufferUsageHint.StaticDraw);

            m_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(m_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            m_elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, m_vetices.Count * sizeof(uint), m_indeces.ToArray(), BufferUsageHint.StaticDraw);
        }

        public void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(m_vertexBufferObject);
            GL.DeleteVertexArray(m_vertexArrayObject);
        }

        private float m_t = 0;

        public void OnUpdateFrame(FrameEventArgs args)
        {
            m_t += (float)args.Time;
            m_worldTransform = Matrix4.Identity; //Matrix4.CreateTranslation(0f, MathF.Sin(m_t), 0);
        }

        public void OnRenderFrame(ref Shader shader)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.SetUniform("uModelMtx", m_worldTransform);
            GL.BindVertexArray(m_vertexArrayObject);

            shader.Use();
            //GL.DrawArrays(PrimitiveType.Triangles, 0 , 3);
            GL.DrawElements(PrimitiveType.Triangles, m_indeces.Count, DrawElementsType.UnsignedInt, 0);
        }

        private List<float> m_vetices = new List<float>();
        private List<float> m_normals = new List<float>();
        private List<int> m_indeces = new List<int>();
        private List<float> m_textureCoordinates = new List<float>();

        private int m_vertexBufferObject;
        private int m_vertexArrayObject;
        private int m_elementBufferObject;

        private Matrix4 m_worldTransform;


    }
}
