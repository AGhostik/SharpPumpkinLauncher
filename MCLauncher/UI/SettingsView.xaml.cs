using System;
using System.Windows;

namespace MCLauncher.UI
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public void CloseSettingsEvent(object sender, EventArgs args)
        {
            Close();
        }
    }
}