using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Testy
{
    public class Shader : IDisposable
    {
        public int Handle => m_handle;

        private readonly Dictionary<string, int> m_uniformLocations;
        private readonly Dictionary<string, ActiveUniformType> m_uniformTypes;

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            {
                GL.CompileShader(vertexShader);
                GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(vertexShader);
                    Console.WriteLine(infoLog);
                }
            }
            
            {
                GL.CompileShader(fragmentShader);
                GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(fragmentShader);
                    Console.WriteLine(infoLog);
                }
            }

            m_handle = GL.CreateProgram();

            GL.AttachShader(m_handle, vertexShader);
            GL.AttachShader(m_handle, fragmentShader);

            GL.LinkProgram(m_handle);
            {
                GL.GetProgram(m_handle, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(m_handle);
                    Console.WriteLine(infoLog);
                }
            }

            GL.DetachShader(m_handle, vertexShader);
            GL.DetachShader(m_handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            m_uniformLocations = new Dictionary<string, int>();
            m_uniformTypes = new Dictionary<string, ActiveUniformType>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out var type);
                var loc = GL.GetUniformLocation(Handle, key);
                
                m_uniformLocations.Add(key, loc);
                m_uniformTypes.Add(key, type);
            }
        }

        public void Use()
        {
            GL.UseProgram(m_handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                GL.DeleteProgram(m_handle);

                m_disposed = true;
            }
        }

        public void SetUniform<T>(string name, T value)
        {
            GL.UseProgram(Handle);
            switch (value)
            {
                case int i:
                    if (m_uniformTypes[name] != ActiveUniformType.Int)
                        throw new ArgumentException();
                    GL.Uniform1(m_uniformLocations[name], i);
                    break;
                case float f:
                    if (m_uniformTypes[name] != ActiveUniformType.Float)
                        throw new ArgumentException();
                    GL.Uniform1(m_uniformLocations[name], f);
                    break;
                case Matrix4 mtx:
                    if (m_uniformTypes[name] != ActiveUniformType.FloatMat4)
                        throw new ArgumentException();
                    GL.UniformMatrix4(m_uniformLocations[name], true, ref mtx);
                    break;
                case Vector3 v3:
                    if (m_uniformTypes[name] != ActiveUniformType.FloatVec3)
                        throw new ArgumentException();
                    GL.Uniform3(m_uniformLocations[name], v3);
                    break;

            }
        }
        
        public bool TrySetUniform<T>(string name, T value)
        {
            GL.UseProgram(Handle);
            switch (value)
            {
                case int i:
                    if (!m_uniformTypes.ContainsKey(name) ||m_uniformTypes[name] != ActiveUniformType.Int)
                        return false;
                    GL.Uniform1(m_uniformLocations[name], i);
                    return true;
                    break;
                case float f:
                    if (!m_uniformTypes.ContainsKey(name) ||m_uniformTypes[name] != ActiveUniformType.Float)
                        return false;
                    GL.Uniform1(m_uniformLocations[name], f);
                    return true;
                    break;
                case Matrix4 mtx:
                    if (!m_uniformTypes.ContainsKey(name) ||m_uniformTypes[name] != ActiveUniformType.FloatMat4)
                        return false;
                    GL.UniformMatrix4(m_uniformLocations[name], true, ref mtx);
                    return true;
                    break;
                case Vector3 v3:
                    if (!m_uniformTypes.ContainsKey(name) || m_uniformTypes[name] != ActiveUniformType.FloatVec3)
                        return false;
                    GL.Uniform3(m_uniformLocations[name], v3);
                    return true;
                    break;

            }

            return false;
        }

        private int m_handle;
        private bool m_disposed;
    }
}
