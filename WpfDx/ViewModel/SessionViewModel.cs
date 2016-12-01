﻿using System;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using OlivecDx;
using OlivecDx.Render;
using WpfDx.Model;
using System.Threading;

namespace WpfDx.ViewModel
{
  public class SessionViewModel : ViewModelBase
  {
    private readonly OlivecDx.View _view;
    public SessionViewModel()
    {

      var loader = new MeshLoader();
      var m = new Mesh(loader.Load("data/opened_shell.txt"));

      var triangles = new Triangles(m.Vertices, m.VertexNormals, m.Faces);
      var obj = new SceneObject(triangles, Matrix4x4.Identity);
      var scene = new Scene();
      scene.AddObject(obj);


    //_view.Position = new Vector3(0, 0, -50);
//      view.Direction = new Vector3(0, 0, 0);
      //view.Up = Vector3.UnitY;

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
      Surface.AddDirtyRect(new Int32Rect(0, 0, Surface.PixelWidth, Surface.PixelHeight));
      Surface.Unlock();

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
