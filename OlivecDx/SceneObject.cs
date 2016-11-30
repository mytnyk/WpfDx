using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OlivecDx.Render;

namespace OlivecDx
{
  public class SceneObject
  {
    private Matrix4x4 _position;
    private Triangles _triangles;

    public SceneObject(Triangles triangles, Matrix4x4 position)
    {
      _triangles = triangles;
      _position = position;
    }
  }
}
