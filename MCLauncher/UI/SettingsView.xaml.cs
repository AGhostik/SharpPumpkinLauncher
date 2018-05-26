using System.Windows;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}