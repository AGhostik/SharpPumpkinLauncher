using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MCLauncher.Model
{
    [Serializable]
    public class Profile : ObservableObject
    {
        private string _currentVersion;
        private string _gameDirectory;
        private string _javaFile;
        private string _jvmArgs;
        private LauncherVisibility _launcherVisibility;
        private string _name;
        private string _nickname;
        private bool _showAlpha;
        private bool _showBeta;
        private bool _showCustom;
        private bool _showRelease;
        private bool _showSnapshot;

        public Profile()
        {
            Name = string.Empty;
            Nickname = string.Empty;
            JavaFile = string.Empty;
            GameDirectory = AppDomain.CurrentDomain.BaseDirectory + "Minecraft";
            JvmArgs = string.Empty;
            LauncherVisibility = LauncherVisibility.KeepOpen;
            CurrentVersion = string.Empty;
            ShowCustom = false;
            ShowRelease = true;
            ShowSnapshot = false;
            ShowBeta = false;
            ShowAlpha = false;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Nickname
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }

        public string JavaFile
        {
            get => _javaFile;
            set => SetProperty(ref _javaFile, value);
        }

        public string GameDirectory
        {
            get => _gameDirectory;
            set => SetProperty(ref _gameDirectory, value);
        }

        public string JvmArgs
        {
            get => _jvmArgs;
            set => SetProperty(ref _jvmArgs, value);
        }

        public string CurrentVersion
        {
            get => _currentVersion;
            set => SetProperty(ref _currentVersion, value);
        }

        public LauncherVisibility LauncherVisibility
        {
            get => _launcherVisibility;
            set => SetProperty(ref _launcherVisibility, value);
        }

        public bool ShowCustom
        {
            get => _showCustom;
            set
            {
                SetProperty(ref _showCustom, value);
                SelectedVersionsChanged?.Invoke(this, null);
            }
        }

        public bool ShowRelease
        {
            get => _showRelease;
            set
            {
                SetProperty(ref _showRelease, value);
                SelectedVersionsChanged?.Invoke(this, null);
            }
        }

        public bool ShowSnapshot
        {
            get => _showSnapshot;
            set
            {
                SetProperty(ref _showSnapshot, value);
                SelectedVersionsChanged?.Invoke(this, null);
            }
        }

        public bool ShowBeta
        {
            get => _showBeta;
            set
            {
                SetProperty(ref _showBeta, value);
                SelectedVersionsChanged?.Invoke(this, null);
            }
        }

        public bool ShowAlpha
        {
            get => _showAlpha;
            set
            {
                SetProperty(ref _showAlpha, value);
                SelectedVersionsChanged?.Invoke(this, null);
            }
        }

        public event EventHandler SelectedVersionsChanged;
    }
}