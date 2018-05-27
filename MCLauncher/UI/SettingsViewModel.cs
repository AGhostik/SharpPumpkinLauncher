using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MCLauncher.Model;
using MCLauncher.Properties;

namespace MCLauncher.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Installer _installer;
        private string _gameDirectory;
        private string _javaFile;
        private string _jvmArgs;
        private LauncherVisibility _launcherVisibility;
        private string _minecraftVersion;
        private string _name;
        private string _nickname;
        private string _selectedVisibility;
        private bool _showAlpha;
        private bool _showBeta;
        private bool _showCustom;
        private bool _showRelease;
        private bool _showSnapshot;

        public SettingsViewModel(Installer installer)
        {
            _installer = installer;
            _init();
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Nickname
        {
            get => _nickname;
            set => Set(ref _nickname, value);
        }

        public string JavaFile
        {
            get => _javaFile;
            set => Set(ref _javaFile, value);
        }

        public string GameDirectory
        {
            get => _gameDirectory;
            set => Set(ref _gameDirectory, value);
        }

        public string JvmArgs
        {
            get => _jvmArgs;
            set => Set(ref _jvmArgs, value);
        }

        public string MinecraftVersion
        {
            get => _minecraftVersion;
            set => Set(ref _minecraftVersion, value);
        }

        public bool ShowCustom
        {
            get => _showCustom;
            set
            {
                Set(ref _showCustom, value);
                _fillVersions();
            }
        }

        public bool ShowRelease
        {
            get => _showRelease;
            set
            {
                Set(ref _showRelease, value);
                _fillVersions();
            }
        }

        public bool ShowSnapshot
        {
            get => _showSnapshot;
            set
            {
                Set(ref _showSnapshot, value);
                _fillVersions();
            }
        }

        public bool ShowBeta
        {
            get => _showBeta;
            set
            {
                Set(ref _showBeta, value);
                _fillVersions();
            }
        }

        public bool ShowAlpha
        {
            get => _showAlpha;
            set
            {
                Set(ref _showAlpha, value);
                _fillVersions();
            }
        }

        public string SelectedVisibility
        {
            get => _selectedVisibility;
            set
            {
                Set(ref _selectedVisibility, value);

                if (_selectedVisibility == UIResource.KeepLauncherOpen)
                    _launcherVisibility = LauncherVisibility.KeepOpen;
                else if (_selectedVisibility == UIResource.HideLauncher)
                    _launcherVisibility = LauncherVisibility.Hide;
                else if (_selectedVisibility == UIResource.CloseLauncher)
                    _launcherVisibility = LauncherVisibility.Close;
            }
        }

        public List<string> Visibilitys { get; } = new List<string>
        {
            UIResource.KeepLauncherOpen,
            UIResource.HideLauncher,
            UIResource.CloseLauncher
        };

        public ObservableCollection<string> Versions { get; set; } = new ObservableCollection<string>();

        public RelayCommand Save { get; set; }
        public RelayCommand Cancel { get; set; }

        private void _init()
        {
            _loadProfile();
            _fillVersions();

            Save = new RelayCommand(() =>
            {
                //
                _saveProfile();
            });
            Cancel = new RelayCommand(() =>
            {
                //
            });
        }

        private void _fillVersions()
        {
            Versions.Clear();
            var versions = _installer.GetVersions(ShowCustom, ShowRelease, ShowSnapshot, ShowBeta, ShowAlpha);
            foreach (var version in versions) Versions.Add(version);
        }

        private void _saveProfile()
        {
            Settings.Default.LastProfile.Name = Name;
            Settings.Default.LastProfile.Nickname = Nickname;
            Settings.Default.LastProfile.JavaFile = JavaFile;
            Settings.Default.LastProfile.GameDirectory = GameDirectory;
            Settings.Default.LastProfile.JvmArgs = JvmArgs;
            Settings.Default.LastProfile.LauncherVisibility = _launcherVisibility;
            Settings.Default.LastProfile.MinecraftVersion = MinecraftVersion;
            Settings.Default.LastProfile.ShowCustom = ShowCustom;
            Settings.Default.LastProfile.ShowRelease = ShowRelease;
            Settings.Default.LastProfile.ShowSnapshot = ShowSnapshot;
            Settings.Default.LastProfile.ShowBeta = ShowBeta;
            Settings.Default.LastProfile.ShowAlpha = ShowAlpha;

            Settings.Default.Save();

            MessageBox.Show("Done!");
        }

        private void _loadProfile()
        {
            if (Settings.Default.LastProfile == null)
                Settings.Default.LastProfile = new Profile();

            var profile = Settings.Default.LastProfile;

            Name = profile.Name;
            Nickname = profile.Nickname;
            JavaFile = profile.JavaFile;
            GameDirectory = profile.GameDirectory;
            JvmArgs = profile.JvmArgs;
            _launcherVisibility = profile.LauncherVisibility;
            MinecraftVersion = profile.MinecraftVersion;
            ShowCustom = profile.ShowCustom;
            ShowRelease = profile.ShowRelease;
            ShowSnapshot = profile.ShowSnapshot;
            ShowBeta = profile.ShowBeta;
            ShowAlpha = profile.ShowAlpha;
        }
    }
}