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
    public static Vector3 Convert(this ObjVector3 inVector)
    {
        return new Vector3(inVector.X, inVector.Y, inVector.Z);
    }
}