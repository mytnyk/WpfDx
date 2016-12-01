using System.Collections.Generic;

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
