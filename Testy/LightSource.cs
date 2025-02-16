using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Testy;

public class LightSource : ObjectBase
{
    public Color4 Colour => m_colour;
    
    public LightSource(Vector3 position, Color4 colour) : base(position, Quaternion.Identity, colour)
    {
        
    }
    
    public override void OnLoad()
    {
        // m_vertexArrayObject = GL.GenVertexArray();
        // GL.BindVertexArray(m_vertexArrayObject);
        //
        // GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBufferObject);
        
        
    }

    public override void OnUnload()
    {
    }

    public override void OnUpdateFrame(FrameEventArgs args)
    {
    }

    public override void OnRenderFrame(ref Shader shader)
    {
    }
}