using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using StbImageSharp;

namespace Testy
{
    internal class RenderableObject : ObjectBase
    {
        public RenderableObject(OBJLibrary.OBJMesh mesh, MaterialLibrary.Material material, string texturePath, Vector3 position,
            Quaternion rotation, Color4 color) : base(position, rotation, color)
        {
            // Expanded buffers
            m_vertices = mesh.Vertices;
            m_normals = mesh.Normals;
            m_vertexColours = mesh.VertexColours;
            m_indices = mesh.Indices;
            m_textureCoordinates = mesh.TexCoords;
            
            m_material = material;
            
            m_diffuseTexPath = texturePath;
        }

        public override void OnLoad()
        {
            m_diffuseTexture = Texture.LoadFromFile(m_diffuseTexPath);
            
            // VERTEX
            var verticesArray = m_vertices.SelectMany(v => new float[] {v.X, v.Y, v.Z}).ToArray();
            
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
            
            // TEXTURE COORDINATES
            var textureCoordsArray = m_textureCoordinates.SelectMany(tc => new float[] { tc.X, tc.Y }).ToArray();
            
            m_bufferObject[TEXTURE_BUFFER] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferObject[TEXTURE_BUFFER]);
            GL.BufferData(BufferTarget.ArrayBuffer, textureCoordsArray.Length * sizeof(float), textureCoordsArray, BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(TEXTURE_BUFFER, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(TEXTURE_BUFFER);
            
            
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
            m_diffuseTexture.Use(TextureUnit.Texture0);
            shader.TrySetUniform("material.diffuse", 0);
            shader.TrySetUniform("material.specularColour", m_material.Specular);
            shader.TrySetUniform("material.shininess", m_material.Shininess);
            GL.BindVertexArray(m_vertexArrayObject);
            
            GL.DrawElements(PrimitiveType.Triangles, m_indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
        
        private void WeldVertices()
        {
            var uniqueVertices = new Dictionary<Vector3, int>();
            var newVertices = new List<Vector3>();
            var newIndices = new List<int>();

            foreach (var index in m_indices)
            {
                var vertex = m_vertices[index];

                if (!uniqueVertices.TryGetValue(vertex, out int newIndex))
                {
                    newIndex = newVertices.Count;
                    uniqueVertices[vertex] = newIndex;
                    newVertices.Add(vertex);
                }

                newIndices.Add(newIndex);
            }

            m_vertices = newVertices;
            m_indices = newIndices;
        }

        private void SmoothNormals()
        {
            var normals = Enumerable.Repeat(Vector3.Zero, m_vertices.Count).ToList();
            var counts = new int[m_vertices.Count];
            
            for (var i = 0; i < m_indices.Count; i += 3)
            {
                var i0 = m_indices[i];
                var i1 = m_indices[i + 1];
                var i2 = m_indices[i + 2];
                
                var v0 = m_vertices[i0];
                var v1 = m_vertices[i1];
                var v2 = m_vertices[i2];
                
                var normal = Vector3.Cross(v1 - v0, v2 - v0).Normalized();

                normals[i0] += normal;
                normals[i1] += normal;
                normals[i2] += normal;
                
                counts[i0]++;
                counts[i1]++;
                counts[i2]++;
            }

            for (int i = 0; i < normals.Count; i++)
            {
                if (counts[i] > 0)
                    normals[i] = normals[i].Normalized();
            }

            m_normals = normals.ToList();
        }
        
        private List<Vector3> m_vertices;
        private List<Vector3> m_normals;
        private List<int> m_indices;
        private readonly List<float> m_vertexColours;
        private readonly List<Vector2> m_textureCoordinates;
        private Texture m_diffuseTexture;
        
        private readonly MaterialLibrary.Material m_material;
        private readonly string m_diffuseTexPath;
    }
}
