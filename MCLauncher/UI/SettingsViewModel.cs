using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly bool _isNewProfile;
        private readonly SettingsModel _settingsModel;
        private string _oldProfileName;
        private string _selectedVisibility;

        private Versions _versions;

        public SettingsViewModel(SettingsModel model, bool isNewProfile)
        {
            _settingsModel = model;
            _isNewProfile = isNewProfile;
            _init();
        }

        public Profile CurrentProfile { get; set; }

        public string SelectedVisibility
        {
            get => _selectedVisibility;
            set
            {
                Set(ref _selectedVisibility, value);

                if (_selectedVisibility == UIResource.KeepLauncherOpen)
                    CurrentProfile.LauncherVisibility = LauncherVisibility.KeepOpen;
                else if (_selectedVisibility == UIResource.HideLauncher)
                    CurrentProfile.LauncherVisibility = LauncherVisibility.Hide;
                else if (_selectedVisibility == UIResource.CloseLauncher)
                    CurrentProfile.LauncherVisibility = LauncherVisibility.Close;
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
        public RelayCommand OpenDirectory { get; set; }

        public string Title { get; set; }

        private void _init()
        {
            _loadProfile();
            _getVersions();
            _fillVersions();

            Title = _isNewProfile ? UIResource.NewProfileTitle : UIResource.EditProfileTitle;

            CurrentProfile.VersionsReload += (sender, args) => { _fillVersions(); };

            Save = new RelayCommand(_saveProfile);
            OpenDirectory = new RelayCommand(() => { _settingsModel.OpenGameDirectory(CurrentProfile.GameDirectory); });
        }

        public void _fillVersions()
        {
            Versions.Clear();
            if (CurrentProfile.ShowRelease)
                foreach (var version in _versions.Release)
                    Versions.Add(version);
            if (CurrentProfile.ShowSnapshot)
                foreach (var version in _versions.Snapshot)
                    Versions.Add(version);
            if (CurrentProfile.ShowBeta)
                foreach (var version in _versions.Beta)
                    Versions.Add(version);
            if (CurrentProfile.ShowAlpha)
                foreach (var version in _versions.Alpha)
                    Versions.Add(version);
        }

        private void _saveProfile()
        {
            if (_isNewProfile)
                _settingsModel.SaveProfile(CurrentProfile);
            else
                _settingsModel.EditProfile(_oldProfileName, CurrentProfile);

            Messenger.Default.Send(new object());
        }

        private void _getVersions()
        {
            _versions = _settingsModel.GetVersions();
        }

        private void _loadProfile()
        {
            CurrentProfile = _settingsModel.LoadLastProfile() ?? new Profile();

            _oldProfileName = CurrentProfile.Name;

            switch (CurrentProfile.LauncherVisibility)
            {
                case LauncherVisibility.KeepOpen:
                    _selectedVisibility = UIResource.KeepLauncherOpen;
                    break;
                case LauncherVisibility.Hide:
                    _selectedVisibility = UIResource.HideLauncher;
                    break;
                case LauncherVisibility.Close:
                    _selectedVisibility = UIResource.CloseLauncher;
                    break;
            }
        }
    }
}