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
    public readonly List<SceneObject> ListOfObjects = new List<SceneObject>();
    public void AddObject(SceneObject obj)
    {
      ListOfObjects.Add(obj);
    }
  }
}
