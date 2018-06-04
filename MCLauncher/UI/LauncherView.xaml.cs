using System.Windows;

namespace MCLauncher.UI
{
    public partial class LauncherView : Window
    {
        public LauncherView(LauncherViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}