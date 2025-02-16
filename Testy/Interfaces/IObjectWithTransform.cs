using OpenTK.Mathematics;

namespace Testy.Interfaces;

public interface IObjectWithTransform
{
    Matrix4 GetTransform();
}