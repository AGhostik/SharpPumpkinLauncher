using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Installer _installer;
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

        public string Name { get; set; }
        public string Nickname { get; set; }
        public string JavaFile { get; set; }
        public string GameDirectory { get; set; }
        public string JvmArgs { get; set; }
        public LauncherVisibility LauncherVisibility { get; set; }
        public string MinecraftVersion { get; set; }

        public bool ShowCustom
        {
            get => _showCustom;
            set
            {
                _showCustom = value;
                _fillVersions();
            }
        }

        public bool ShowRelease
        {
            get => _showRelease;
            set
            {
                _showRelease = value;
                _fillVersions();
            }
        }

        public bool ShowSnapshot
        {
            get => _showSnapshot;
            set
            {
                _showSnapshot = value;
                _fillVersions();
            }
        }

        public bool ShowBeta
        {
            get => _showBeta;
            set
            {
                _showBeta = value;
                _fillVersions();
            }
        }

        public bool ShowAlpha
        {
            get => _showAlpha;
            set
            {
                _showAlpha = value;
                _fillVersions();
            }
        }

        public string SelectedVisibility
        {
            get => _selectedVisibility;
            set
            {
                _selectedVisibility = value;

                if (_selectedVisibility == UIResource.KeepLauncherOpen)
                    LauncherVisibility = LauncherVisibility.KeepOpen;
                else if (_selectedVisibility == UIResource.HideLauncher)
                    LauncherVisibility = LauncherVisibility.Hide;
                else if (_selectedVisibility == UIResource.CloseLauncher) LauncherVisibility = LauncherVisibility.Close;
            }
        }

        public List<string> Visibilitys { get; private set; }
        public ObservableCollection<string> Versions { get; set; } = new ObservableCollection<string>();

        private void _init()
        {
            _fillVersions();
            Visibilitys = new List<string>
            {
                UIResource.KeepLauncherOpen,
                UIResource.HideLauncher,
                UIResource.CloseLauncher
            };
        }

        private void _fillVersions()
        {
            Versions.Clear();
            var versions = _installer.GetVersions(ShowCustom, ShowRelease, ShowSnapshot, ShowBeta, ShowAlpha);
            foreach (var version in versions) Versions.Add(version);
        }

        public Profile GetProfile()
        {
            return new Profile
            {
                Name = Name,
                Nickname = Nickname,
                JavaFile = JavaFile,
                GameDirectory = GameDirectory,
                JvmArgs = JvmArgs,
                LauncherVisibility = LauncherVisibility,
                MinecraftVersion = MinecraftVersion,
                ShowCustom = ShowCustom,
                ShowRelease = ShowRelease,
                ShowSnapshot = ShowSnapshot,
                ShowBeta = ShowBeta,
                ShowAlpha = ShowAlpha
            };
        }
    }
}