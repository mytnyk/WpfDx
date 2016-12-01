using System.Windows;
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
      var main_view = new View.MainView {DataContext = main_view_model};
      main_view.Show();
    }
  }
}
