using System.Diagnostics;
using JeremyAnsel.Media.WavefrontObj;
using ObjLoader.Loader.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Testy
{
    internal class RenderableObject : ObjectBase
    {
        public RenderableObject(Shape shape, Vector3 position, Quaternion rotation, Color4 color) : base(position,
            rotation, color)
        {
            m_vetices = shape.Vertices.ToList();
            m_indices = shape.Indices.ToList();
            shape.GenerateNormals();
            m_normals = shape.Normals.ToList();
            //m_normals = shape.Normals.ToList();
        }

        public RenderableObject(string filePath, Vector3 position, Quaternion rotation, Color4 color) : base(position, rotation, color)
        {
            var file = ObjFile.FromFile(filePath);

            // Expanded buffers
            m_vetices = new List<Vector3>();
            m_normals = new List<Vector3>();
            m_vertexColours = new List<float>();
            m_indices = new List<int>();

            int vertIndex = 0;
            foreach (var face in file.Faces)
            {
                foreach (var vertex in face.Vertices)
                {
                    // Add position
                    var pos = file.Vertices[vertex.Vertex - 1];
                    m_vetices.Add(new Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z));

                    // Add normal
                    var normal = file.VertexNormals[vertex.Normal - 1];
                    m_normals.Add(normal.Convert());

                    // Add color
                    m_vertexColours.Add(m_colour.R);
                    m_vertexColours.Add(m_colour.G);
                    m_vertexColours.Add(m_colour.B);

                    // Add index
                    m_indices.Add(vertIndex++);
                }
            }

            m_colour = color;
            m_worldTransform = Matrix4.CreateTranslation(position) * Matrix4.CreateFromQuaternion(rotation);
        }

        public override void OnLoad()
        {
            // VERTEX
            var verticesArray = m_vetices.SelectMany(v => new float[] {v.X, v.Y, v.Z}).ToArray();
            
            m_bufferObject[VERTEX_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[VERTEX_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesArray.Length * sizeof(float), verticesArray, BufferUsageHint.StaticDraw);

            m_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(m_vertexArrayObject);

            GL.VertexAttribPointer(VERTEX_BUFFER, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(VERTEX_BUFFER);
            
            // NORMALS
            var normalsArray = m_normals.SelectMany(v => new float[] {v.X, v.Y, v.Z}).ToArray();
            
            m_bufferObject[NORMAL_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[NORMAL_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, normalsArray.Length * sizeof(float), normalsArray.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(NORMAL_BUFFER, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(NORMAL_BUFFER);
            
            // COLOUR
            
            m_bufferObject[COLOUR_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[COLOUR_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vertexColours.Count * sizeof(float), m_vertexColours.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(COLOUR_BUFFER, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(COLOUR_BUFFER);
            
            // INDICES
            
            m_bufferObject[INDEX_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_bufferObject[INDEX_BUFFER]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, m_indices.Count * sizeof(uint), m_indices.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(INDEX_BUFFER, 1, VertexAttribPointerType.UnsignedInt, false, sizeof(uint), 0);
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
            shader.SetUniform("uModelMtx", m_worldTransform);
            GL.BindVertexArray(m_vertexArrayObject);
            
            GL.DrawElements(PrimitiveType.Triangles, m_vetices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private List<Vector3> m_vetices = new List<Vector3>();
        private List<Vector3> m_normals = new List<Vector3>();
        private List<int> m_indices = new List<int>();
        private List<float> m_vertexColours = new List<float>();
        private List<float> m_textureCoordinates = new List<float>();
        
        private int m_elementBufferObject;
    }
}
