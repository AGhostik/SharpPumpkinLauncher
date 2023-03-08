namespace MCLauncher.UI;

public partial class LauncherView
{
    public LauncherView(LauncherViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}