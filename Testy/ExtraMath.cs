using JeremyAnsel.Media.WavefrontObj;
using OpenTK.Mathematics;

namespace Testy;

public static class ExtraMath
{
    public static double TwoPI = System.Math.PI * 2.0;
}

public static class ExtraMathF
{
    public const float TwoPI = System.MathF.PI * 2.0f;
}

public static class Utils
{
    public static Vector3 ToOpenTkVec3(this ObjVector3 inVector)
    {
        return new Vector3(inVector.X, inVector.Y, inVector.Z);
    }
    
    public static System.Numerics.Vector3 ToSystemVec3(this Vector3 inVector)
    {
        return new System.Numerics.Vector3(inVector.X, inVector.Y, inVector.Z);
    }
    
    public static Vector3 ToOpenTkVec3(this System.Numerics.Vector3 inVector)
    {
        return new Vector3(inVector.X, inVector.Y, inVector.Z);
    }
}