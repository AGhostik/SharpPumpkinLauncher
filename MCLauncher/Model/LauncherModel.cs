using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.UI;
using MCLauncher.UI.Messages;

namespace MCLauncher.Model
{
    public class LauncherModel : ILauncherModel
    {
        private readonly IFileManager _fileManager;
        private readonly IInstaller _installer;

        public LauncherModel(IInstaller installer, IFileManager fileManager)
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
            Action exitedAction = MinecraftProcessExited;
            
            switch (profile.LauncherVisibility)
            {
                case LauncherVisibility.Close:
                    Application.Current?.MainWindow?.Close();
                    break;
                case LauncherVisibility.Hide:
                    Application.Current?.MainWindow?.Hide();
                    exitedAction = () =>
                    {
                        ShowMainWindow();
                        MinecraftProcessExited();
                    };
                    break;
                case LauncherVisibility.KeepOpen:
                    break;
            }

            Messenger.Default.Send(new StatusMessage(UIResource.LaunchGameStatus));

            _fileManager.StartProcess(profile.JavaFile, _installer.LaunchArgs, exitedAction);
        }

        private static void MinecraftProcessExited()
        {
            Messenger.Default.Send(new StatusMessage(UIResource.GameExitedStatus));
        }

        private static void ShowMainWindow()
        {
            Messenger.Default.Send(new MinecraftExitedMessage());
        }
    }
}