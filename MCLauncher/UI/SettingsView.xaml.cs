using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.UI.Messages;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsView(SettingsViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<ProfilesChangedMessage>(this, (r, message) => { Close(); });
        }

        private async void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.Init();
        }
    }
}