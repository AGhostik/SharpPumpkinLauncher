using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();

            Messenger.Default.Register(this, (object dummy) => {
                _close();
            });
        }

        private void _close()
        {
            Close();
        }
    }
}