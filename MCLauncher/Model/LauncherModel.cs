using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model.Managers;
using MCLauncher.UI;
using MCLauncher.UI.Messages;

namespace MCLauncher.Model
{
    public class LauncherModel : ILauncherModel
    {
        private readonly IFileManager _fileManager;
        private readonly IProfileManager _profileManager;
        private readonly IInstaller _installer;

        public LauncherModel(IInstaller installer, IProfileManager profileManager, IFileManager fileManager)
        {
            _installer = installer;
            _profileManager = profileManager;
            _fileManager = fileManager;
        }

        public async Task StartGame()
        {
            var currentProfile = _profileManager.GetLast();
            await _installer.Install(currentProfile);
            _launchMinecraft(currentProfile);
        }

        public void SaveLastProfileName(string name)
        {
            _profileManager.SaveLastProfileName(name);
        }

        public void OpenNewProfileWindow()
        {
            Messenger.Default.Send(new ShowSettingsMessage(true));
        }

        public void OpenEditProfileWindow()
        {
            Messenger.Default.Send(new ShowSettingsMessage(false));
        }

        public void DeleteProfile(string name)
        {
            _profileManager.Delete(name);
            Messenger.Default.Send(new ProfilesChangedMessage());
        }

        public List<string> GetProfiles()
        {
            var names = new List<string>();
            var profiles = _profileManager.GetProfiles();

            foreach (var profile in profiles) names.Add(profile.Name);

            return names;
        }

        public string GetLastProfile()
        {
            var lastProfile = _profileManager.GetLastProfileName();
            return lastProfile;
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