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
    private readonly List<SceneObject> _list_of_objects = new List<SceneObject>();
    public void AddObject(SceneObject obj)
    {
      _list_of_objects.Add(obj);
    }
  }
}
