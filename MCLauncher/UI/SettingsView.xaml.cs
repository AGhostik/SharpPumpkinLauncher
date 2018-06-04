using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.UI.Messages;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
            InitializeComponent();

            Messenger.Default.Register(this, (ProfilesChangedMessage message) => { Close(); });
        }
    }
}