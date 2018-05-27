using System;
using System.Windows;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        private readonly SettingsViewModel _settingsViewModel;

        public SettingsView(SettingsViewModel settingsViewModel)
        {
            _settingsViewModel = settingsViewModel;
            DataContext = _settingsViewModel;
            _settingsViewModel.CloseSettingsEvent += _closeSettingsEvent;
            InitializeComponent();
        }

        private void _closeSettingsEvent(object sender, EventArgs args)
        {
            _settingsViewModel.CloseSettingsEvent -= _closeSettingsEvent;
            Close();
        }
    }
}