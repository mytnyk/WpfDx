using System.Numerics;
using OlivecDx.Tex;
using SharpDX;
using SharpDX.Direct3D10;

namespace OlivecDx
{
  public class SceneObject
  {
    private readonly Matrix _position;
    private readonly Triangles _triangles;

    private static Matrix _Convert(Matrix4x4 transform)
    {
      return new Matrix(new[]
      {
        transform.M11, transform.M12, transform.M13, transform.M14,
        transform.M21, transform.M22, transform.M23, transform.M24,
        transform.M31, transform.M32, transform.M33, transform.M34,
        transform.M41, transform.M42, transform.M43, transform.M44,
      });
    }
    public SceneObject(Triangles triangles, Matrix4x4 position)
    {
      _triangles = triangles;
      _position = _Convert(position);
    }

    public void InitBuffers(Device1 device10)
    {
      _triangles.InitBuffers(device10);
    }

    public void Render(Device1 device10, Matrix viewTransform, Matrix projectionTransform)
    {
      _triangles.Render(device10, viewTransform, projectionTransform, _position);
    }
  }
}
