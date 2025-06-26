using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Testy.Interfaces;

namespace Testy;

public abstract class ObjectBase : IObjectWithTransform
{
    // A bit annoying and hacky really but I don't want to have to cast every time I use it...
    public const int VERTEX_BUFFER = (int)BufferTypes.Vertex;
    public const int COLOUR_BUFFER = (int)BufferTypes.Colour;
    public const int NORMAL_BUFFER = (int)BufferTypes.Normal;
    public const int INDEX_BUFFER = (int)BufferTypes.Index;
    public const int MAX_BUFFER = (int)BufferTypes.MAX_BUFFER;
    
    private enum BufferTypes
    {
        Vertex = 0,
        Colour = 1,
        Normal = 2,
        Index = 3,
        MAX_BUFFER = 4
    }
    
    public ObjectBase(Vector3 position, Quaternion rotation, Color4 colour)
    {
        m_colour = colour;
        m_worldTransform = Matrix4.CreateTranslation(position) * Matrix4.CreateFromQuaternion(rotation);
    }
    
    public Matrix4 GetTransform()
    {
        return m_worldTransform;
    }

    public Vector3 Position
    {
        get => m_worldTransform.ExtractTranslation();
        set => m_worldTransform = Matrix4.CreateTranslation(value) * Matrix4.CreateFromQuaternion(m_worldTransform.ExtractRotation());
    }        

    public abstract void OnLoad();
    public abstract void OnUnload();

    public abstract void OnUpdateFrame(FrameEventArgs args);

    public abstract void OnRenderFrame(ref Shader shader);
    
    protected int[] m_bufferObject = new int[MAX_BUFFER];
    protected int m_vertexArrayObject;
    protected Color4 m_colour;
    protected Matrix4 m_worldTransform = Matrix4.Identity;
}