using System;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using OlivecDx;
using System.Threading;
using Map3dConstructor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OlivecDx.Tex;

namespace WpfDx.ViewModel
{
  public class SessionViewModel : ViewModelBase
  {
    private readonly OlivecDx.View _view;
    public SessionViewModel()
    {

      List<Triangle> walls = new List<Triangle>();
      List<Triangle> floor = new List<Triangle>();

      ElementType[] map = { ElementType.Road, ElementType.Wall, ElementType.Road, ElementType.Road };
      var map2d = new Map3DConstructor(map, 2, 2);
      map2d.Generate(walls, floor, 8);

      var wall_triangles = new Triangles(
        walls.SelectMany(t => new[] { t.A, t.B, t.C }),
        walls.SelectMany(t => new[] { t.At, t.Bt, t.Ct}),
        File.ReadAllBytes("Data/GeneticaMortarlessBlocks.jpg"));
      var wall_obj = new SceneObject(wall_triangles, Matrix4x4.Identity);

      var floor_triangles = new Triangles(
        floor.SelectMany(t => new[] { t.A, t.B, t.C }),
        floor.SelectMany(t => new[] { t.At, t.Bt, t.Ct }),
        File.ReadAllBytes("Data/mud.bmp"));
      var floor_obj = new SceneObject(floor_triangles, Matrix4x4.Identity);


      var gold = Map3DConstructor.GenerateWall(1*8, 2*8, 1*8, 1*8, 8);
      var gold_triangles = new Triangles(
        gold.SelectMany(t => new[] { t.A, t.B, t.C }),
        gold.SelectMany(t => new[] { t.At, t.Bt, t.Ct}),
        File.ReadAllBytes("Data/gold1.png"));
        //File.ReadAllBytes("Data/mud.bmp"));
      var gold_obj = new SceneObject(gold_triangles, Matrix4x4.Identity);

      var scene = new Scene();
      scene.AddObject(wall_obj);
      scene.AddObject(floor_obj);
      //scene.AddObject(gold_obj);

      _view = new OlivecDx.View(600, 600)
      {
        Scene = scene,
        //ViewMatrix = new Matrix4x4()
      };

      _view.InitBuffers();

      CompositionTarget.Rendering += OnRendering;
      Surface = new D3DImage();

      IntPtr backBuffer = _view.GetBackBufferPtr();
      Surface.Lock();
      Surface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, backBuffer);
      Surface.Unlock();

      _view.Position = new Vector3(4, 4, 4);
      _view.Direction = new Vector3(1, 0, 0);

      var shellRotationThread = new Thread(() =>
      {
        while (true)
        {
          _view.TestRotate();
          Thread.Sleep(10);
        }
      });
      shellRotationThread.Start();
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
