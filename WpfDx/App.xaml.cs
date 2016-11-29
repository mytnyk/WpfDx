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

namespace WpfDx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var main_view = new MainView();
            //main_view.Closing += (o, ce) => { main_view_model.Disconnect(); };
            //main_view.DataContext = main_view_model;
            
            main_view.Show();

            IntPtr hwnd = new WindowInteropHelper(main_view).Handle;

            var triangles = new Triangles(null, null, null);
            var scene = new Scene();
            scene.AddObject(triangles);
            var view = new View(hwnd, 100, 100);
            view.Scene = scene;
            view.Position = new Vector3(0, 0, -50);
            view.Direction = new Vector3(0, 0, 0);
            view.Up = Vector3.UnitY;
            view.Run();
        }
    }
}
