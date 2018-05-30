using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using MCLauncher.UI;

namespace MCLauncher.Model
{
    public class MainModel
    {
        private readonly FileManager _fileManager;
        private readonly Installer _installer;

        public MainModel(Installer installer, FileManager fileManager)
        {
            _installer = installer;
            _fileManager = fileManager;
        }

        public void StartGame()
        {
            var currentProfile = _fileManager.GetLastProfile();
            _installer.Install(currentProfile);
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

            foreach (var profile in profiles) names.Add(profile.Name);

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
            mcProcess.Exited += McProcessOnExited;
            mcProcess.Start();

            //launcher visibility
        }

        private static void McProcessOnExited(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("McProcessExited");
        }
    }
}