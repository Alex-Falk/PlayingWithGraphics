using System.Diagnostics;
using ObjLoader.Loader.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Testy
{
    internal class RenderableObject : ObjectBase
    {
        public RenderableObject() : base(Vector3.Zero, Quaternion.Identity, Color4.White)
        {
            m_vetices.AddRange( new []{
                0.5f, 0.5f, 0.0f, // top right
                0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, // bottom left
                -0.5f, 0.5f, 0.0f, // top left
            });
            
            m_indices.AddRange(new []
            {
                0, 1, 3, // The first triangle will be the top-right half of the triangle
                1, 2, 3  // Then the second will be the bottom-left half of the triangle
            });
        }

        public RenderableObject(string filePath, Vector3 position, Quaternion rotation, Color4 color) : base(position, rotation, color)
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
                m_vertexColours.Add(m_colour.R);
                m_vertexColours.Add(m_colour.G);
                m_vertexColours.Add(m_colour.B);
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
                        m_indices.Add(face[i].VertexIndex - 1);
                    }
                }
            }

            m_colour = color;
            m_worldTransform = Matrix4.CreateTranslation(position) * Matrix4.CreateFromQuaternion(rotation);
        }

        public override void OnLoad()
        {
            // VERTEX
            m_bufferObject[VERTEX_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[VERTEX_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vetices.Count * sizeof(float), m_vetices.ToArray(), BufferUsageHint.StaticDraw);

            m_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(m_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(VERTEX_BUFFER);
            
            // NORMALS
            
            m_bufferObject[NORMAL_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[NORMAL_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, m_normals.Count * sizeof(float), m_normals.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(NORMAL_BUFFER);
            
            // COLOUR
            
            m_bufferObject[COLOUR_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[COLOUR_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vertexColours.Count * sizeof(float), m_vertexColours.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(COLOUR_BUFFER);
            
            // INDICES
            
            m_bufferObject[INDEX_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[INDEX_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, m_indices.Count * sizeof(float), m_indices.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(INDEX_BUFFER);
            
            
            GL.GetInteger(GetPName.MaxVertexAttribs, out int maxAttributeCount);
            Debug.WriteLine($"Maximum number of vertex attributes supported: {maxAttributeCount}");

            // m_elementBufferObject = GL.GenBuffer();
            // GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_elementBufferObject);
            // GL.BufferData(BufferTarget.ElementArrayBuffer, m_vetices.Count * sizeof(uint), m_indices.ToArray(), BufferUsageHint.StaticDraw);
        }

        public override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffers(MAX_BUFFER, m_bufferObject);
            GL.DeleteVertexArray(m_vertexArrayObject);
        }

        private float m_t = 0;

        public override void OnUpdateFrame(FrameEventArgs args)
        {
            m_t += (float)args.Time;
            if (m_t > MathHelper.TwoPi)
            {
                m_t = 0;
            }
        }

        public override void OnRenderFrame(ref Shader shader)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.SetUniform("uModelMtx", m_worldTransform);
            GL.BindVertexArray(m_vertexArrayObject);

            //shader.Use();
            //GL.DrawArrays(PrimitiveType.Triangles, 0 , 3);
            //shader.SetUniform("uObjectColor", new Vector3(m_color.R, m_color.G, m_color.B));
            GL.DrawElements(PrimitiveType.Triangles, m_indices.Count, DrawElementsType.UnsignedInt, 0);
            
            GL.BindVertexArray(0);
        }

        private List<float> m_vetices = new List<float>();
        private List<float> m_normals = new List<float>();
        private List<int> m_indices = new List<int>();
        private List<float> m_vertexColours = new List<float>();
        private List<float> m_textureCoordinates = new List<float>();
        
        private int m_elementBufferObject;
    }
}
