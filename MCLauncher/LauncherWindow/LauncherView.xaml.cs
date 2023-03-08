namespace MCLauncher.LauncherWindow;

public partial class LauncherView
{
    public LauncherView(LauncherViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}