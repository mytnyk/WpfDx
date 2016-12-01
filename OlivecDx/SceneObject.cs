using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OlivecDx.Render;
using SharpDX.Direct3D10;

namespace OlivecDx
{
  public class SceneObject
  {
    private Matrix4x4 _position;
    private readonly Triangles _triangles;

    public SceneObject(Triangles triangles, Matrix4x4 position)
    {
      _triangles = triangles;
      _position = position;
    }

    public void InitBuffers(Device1 device10, TrianglesLayout layout)
    {
      _triangles.InitBuffers(device10, layout);
    }

    public void Render(Device1 device10, Matrix4x4 viewTransform, Matrix4x4 projectionTransform)
    {
      _triangles.Render(device10, viewTransform, projectionTransform, _position);
    }
  }
}
