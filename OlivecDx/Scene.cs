using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlivecDx.Render;

namespace OlivecDx
{
  public class Scene
  {
    private List<Triangles> _list_of_objects;
    public void AddObject(Triangles triangles)
    {
      _list_of_objects.Add(triangles);
    }
  }
}
