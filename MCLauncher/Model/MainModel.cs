using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly FileManager _fileManager;
        
        public MainModel(Installer installer, FileManager fileManager)
        {
            _installer = installer;
            _fileManager = fileManager;
        }

        public async Task StartGame()
        {
            var currentProfile = _fileManager.GetLastProfile();
            await _installer.Install(currentProfile);
            _launchMinecraft(currentProfile);
        }

        public void SaveLastProfileName(string name)
        {
            _fileManager.SaveLastProfileName(name);
        }

        public void OpenProfileCreatingWindow()
        {
            _showSettings(true);
        }

        public void OpenProfileEditingWindow()
        {
            _showSettings(false);
        }

        public void DeleteProfile(string name)
        {

        }

        public List<string> GetProfiles()
        {
            var names = new List<string>();
            var profiles = _fileManager.GetProfiles();

            foreach (var profile in profiles)
            {
                names.Add(profile.Name);
            }

            return names;
        }

        public string GetLastProfile()
        {
            var lastProfile = _fileManager.GetLastProfileName();
            return lastProfile;
        }

        private void _showSettings(bool createProfile)
        {
            var settingsModel = new SettingsModel(_fileManager);
            var settingsViewModel = new SettingsViewModel(settingsModel, createProfile);
            var settingsWindow = new SettingsView()
            {
                Owner = Application.Current.MainWindow,
                DataContext = settingsViewModel
            };
            settingsWindow.Show();
        }

        private void _launchMinecraft(Profile profile)
        {
            Debug.WriteLine("launch minecraft");
            var mcProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(profile.JavaFile, _installer.LaunchArgs),
                EnableRaisingEvents = true
            };

            switch (profile.LauncherVisibility)
            {
                case LauncherVisibility.Close:
                    Application.Current.MainWindow.Close();
                    break;
                case LauncherVisibility.Hide: {
                    mcProcess.Exited += MinecraftProcessExited;
                    Application.Current.MainWindow.Hide();
                }
                    break;
                case LauncherVisibility.KeepOpen:
                    break;
            }

            mcProcess.Start();
        }

        private static void MinecraftProcessExited(object sender, EventArgs eventArgs)
        {
            Application.Current.MainWindow.Show();
        }
    }
}
