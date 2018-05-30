using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.UI.Messages;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();

            Messenger.Default.Register(this, (ProfileSavedMessage message) => {
                _close();
            });
        }

        private void _close()
        {
            Close();
        }
    }
}