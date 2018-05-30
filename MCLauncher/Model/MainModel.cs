using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.UI;
using MCLauncher.UI.Messages;

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

        public async Task StartGame()
        {
            Debug.WriteLine("Start");
            var currentProfile = _fileManager.GetLastProfile();
            await _installer.Install(currentProfile);
            Debug.WriteLine("Try launch");
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
            _fileManager.DeleteProfile(name);
            Messenger.Default.Send(new ProfilesChangedMessage());
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
            var mcProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(profile.JavaFile, _installer.LaunchArgs),
                EnableRaisingEvents = true
            };

            mcProcess.Exited += MinecraftProcessExited;

            switch (profile.LauncherVisibility)
            {
                case LauncherVisibility.Close:
                    Application.Current.MainWindow.Close();
                    break;
                case LauncherVisibility.Hide:
                    Application.Current.MainWindow.Hide();
                    mcProcess.Exited += ShowMainWindow;
                    break;
                case LauncherVisibility.KeepOpen:
                    break;
            }

            mcProcess.Start();
        }

        private static void MinecraftProcessExited(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("Mc exited");
        }

        private static void ShowMainWindow(object sender, EventArgs eventArgs)
        {
            Messenger.Default.Send(new MinecraftExitedMessage());
        }
    }
}