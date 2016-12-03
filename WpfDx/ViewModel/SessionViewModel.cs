using System;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using OlivecDx;
using OlivecDx.Render;
using WpfDx.Model;
using System.Threading;
using Map3dConstructor;
using System.Collections.Generic;
using System.Linq;

namespace WpfDx.ViewModel
{
  public class SessionViewModel : ViewModelBase
  {
    private readonly OlivecDx.View _view;
    public SessionViewModel()
    {
/*
      var loader = new MeshLoader();
      var m = new Mesh(loader.Load("data/opened_shell.txt"));
      var triangles = new Triangles(m.Vertices, m.VertexNormals, m.Faces);
      var obj = new SceneObject(triangles, Matrix4x4.Identity);
      var scene = new Scene();
      scene.AddObject(obj);*/

      List<Triangle> walls = new List<Triangle>();
      List<Triangle> floor = new List<Triangle>();

      ElementType[] map = { ElementType.Road, ElementType.Wall, ElementType.Road, ElementType.Road };
      var map2d = new Map2d(map, 2, 2);
      map2d.Generate(walls, floor, 8);

      var wall_mesh = new Mesh(walls.SelectMany(t => new []{t.A, t.B, t.C}).ToArray());
      var wall_triangles = new Triangles(wall_mesh.Vertices, wall_mesh.VertexNormals, wall_mesh.Faces);
      var wall_obj = new SceneObject(wall_triangles, Matrix4x4.Identity);

      var floor_mesh = new Mesh(floor.SelectMany(t => new[] { t.A, t.B, t.C }).ToArray());
      var floor_triangles = new Triangles(floor_mesh.Vertices, floor_mesh.VertexNormals, floor_mesh.Faces);
      var floor_obj = new SceneObject(floor_triangles, Matrix4x4.Identity);

      var scene = new Scene();
      scene.AddObject(wall_obj);
      scene.AddObject(floor_obj);

      _view = new OlivecDx.View(600, 600)
      {
        Scene = scene,
        //ViewMatrix = new Matrix4x4()
      };

      _view.InitBuffers();

      CompositionTarget.Rendering += OnRendering;
      Surface = new D3DImage();
      Surface.IsFrontBufferAvailableChanged += Surface_IsFrontBufferAvailableChanged;

      IntPtr back_buffer = _view.GetBackBufferPtr();
      Surface.Lock();
      Surface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, back_buffer);
      Surface.Unlock();

      _view.Position = new Vector3(4, 4, 4);
      _view.Direction = new Vector3(1, 0, 0);

      var shell_rotation_thread = new Thread(() =>
      {
        while (true)
        {
          _view.TestRotate();
          Thread.Sleep(10);
        }
      });
      shell_rotation_thread.Start();
    }

    private void Surface_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      //throw new NotImplementedException();
    }

    private void OnRendering(object sender, EventArgs e)
    {
      _view.Render();
      Surface.Lock();
      Surface.AddDirtyRect(new Int32Rect(0, 0, Surface.PixelWidth, Surface.PixelHeight));
      Surface.Unlock();
    }

    public D3DImage Surface { get;}

    private ICommand _change_color;

    public ICommand ChangeColorCmd
    {
      get { return _change_color ?? (_change_color = new RelayCommand(param => ChangeColor())); }
    }

    private void ChangeColor()
    {
      _view.SetNewBgColor();
    }
  }
}
