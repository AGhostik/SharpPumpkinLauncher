using System.Windows;
using GalaSoft.MvvmLight.Messaging;
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

            Messenger.Default.Register(this, (ProfilesChangedMessage message) => { Close(); });
        }


        private async void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.Init();
        }
    }
}