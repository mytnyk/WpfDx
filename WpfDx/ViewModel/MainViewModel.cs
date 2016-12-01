namespace WpfDx.ViewModel
{
  public class MainViewModel : ViewModelBase
  {
    public SessionViewModel CurrentSessionViewModel { get; private set; }

    public MainViewModel()
    {
      CurrentSessionViewModel = new SessionViewModel();
    }
  }
}
