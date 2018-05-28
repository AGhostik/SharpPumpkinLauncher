using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MCLauncher.UI;

namespace MCLauncher.Model
{
    public class MainModel
    {
        private readonly Installer _installer;
        private readonly SettingsViewModel _settingsViewModel;

        public MainModel(Installer installer, SettingsViewModel settingsViewModel)
        {
            _installer = installer;
            _settingsViewModel = settingsViewModel;
        }

        public void StartGame()
        {

        }

        public void OpenProfileCreatingWindow()
        {
            _showSettings(UIResource.NewProfileTitle);
        }

        public void OpenProfileEditingWindow()
        {
            _showSettings(UIResource.EditProfileTitle);
        }

        public void DeleteProfile(string name)
        {

        }

        public List<string> GetProfiles()
        {
            var profiles = new List<string>();
            //
            return profiles;
        }

        public string GetLastProfile()
        {
            return "";
        }

        private void _showSettings(string title)
        {
            var settingsWindow = new SettingsView(_settingsViewModel)
            {
                Owner = Application.Current.MainWindow,
                Title = title
            };
            settingsWindow.Show();
        }
    }
}
