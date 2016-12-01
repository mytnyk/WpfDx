using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using OlivecDx;
using OlivecDx.Render;
using WpfDx.Model;
using WpfDx.ViewModel;

namespace WpfDx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

      var main_view_model = new MainViewModel();

      var main_view = new View.MainView();
      main_view.DataContext = main_view_model;
      main_view.Show();
      /*
            IntPtr hwnd = new WindowInteropHelper(main_view).Handle;

            var loader = new MeshLoader();
            var m = new Mesh(loader.Load("data/simple_shell.txt"));

            var triangles = new Triangles(m.Vertices, m.VertexNormals, m.Faces);
            var obj = new SceneObject(triangles, new Matrix4x4());
            var scene = new Scene();
            scene.AddObject(obj);
      
          var view = new OlivecDx.View(hwnd, 100, 100)
          {
            Scene = scene,
            ViewMatrix = new Matrix4x4()
          };
          //view.Position = new Vector3(0, 0, -50);
            //view.Direction = new Vector3(0, 0, 0);
            //view.Up = Vector3.UnitY;
      view.Run();*/
        }
    }
}
