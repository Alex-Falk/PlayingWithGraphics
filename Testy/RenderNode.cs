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
        public RenderableObject(OBJLibrary.OBJMesh mesh, MaterialLibrary.Material material, Vector3 position,
            Quaternion rotation, Color4 color) : base(position, rotation, color)
        {
            // Expanded buffers
            m_vetices = mesh.Vertices;
            m_normals = mesh.Normals;
            m_vertexColours = mesh.VertexColours;
            m_indices = mesh.Indices;
            
            m_material = material;
            
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
            shader.TrySetUniform("material.ambientColour", m_material.Ambient);
            shader.TrySetUniform("material.diffuseColour", m_material.Diffuse);
            shader.TrySetUniform("material.specularColour", m_material.Specular);
            shader.TrySetUniform("material.shininess", 32);
            GL.BindVertexArray(m_vertexArrayObject);
            
            GL.DrawElements(PrimitiveType.Triangles, m_vetices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private readonly List<Vector3> m_vetices;
        private readonly List<Vector3> m_normals;
        private readonly List<int> m_indices;
        private readonly List<float> m_vertexColours = new List<float>();
        private List<float> m_textureCoordinates = new List<float>();
        
        private readonly MaterialLibrary.Material m_material;
    }
}
