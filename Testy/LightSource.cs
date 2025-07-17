using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Testy;

public class LightSource : ObjectBase
{
    public Vector3 AmbientLight { get; set; }
    public Vector3 DiffuseLight { get; set; }
    public Vector3 SpecularLight { get; set; }

    public static LightSource Instance = null;
    
    public static void SetupLightSource(Vector3 position, Color4 colour)
    {
        Instance?.OnUnload();
        Instance = new LightSource(position, colour);
    }
    
    private LightSource(Vector3 position, Color4 colour) : base(position, Quaternion.Identity, colour)
    {
        AmbientLight = 0.5f * new Vector3(colour.R, colour.G, colour.B);
        DiffuseLight = 0.8f * new Vector3(colour.R, colour.G, colour.B);
        SpecularLight = 1.0f * new Vector3(colour.R, colour.G, colour.B);
    }
    
    public override void OnLoad()
    {
    }

    public override void OnUnload()
    {
    }

    public override void OnUpdateFrame(FrameEventArgs args)
    {
    }

    public override void OnRenderFrame(ref Shader shader)
    {
        shader.TrySetUniform("light.ambient",  AmbientLight);
        shader.TrySetUniform("light.diffuse",  DiffuseLight); // darken the light a bit to fit the scene
        shader.TrySetUniform("light.specular", SpecularLight);
        shader.TrySetUniform("light.position", Position);
    }
}