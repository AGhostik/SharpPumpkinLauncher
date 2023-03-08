using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;
using MCLauncher.UI.Messages;

namespace MCLauncher.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsModel _settingsModel;
        private readonly bool _isNewProfile;
        private string _oldProfileName;
        private string _selectedVisibility;

        private AllVersions _versionsCache;
        private Profile _currentProfile;

        public SettingsViewModel(ISettingsModel model, bool isNewProfile)
        {
            _settingsModel = model;
            _isNewProfile = isNewProfile;
        }

        public Profile CurrentProfile
        {
            get => _currentProfile;
            set => Set(ref _currentProfile, value);
        }

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

        public async Task Init()
        {
            var isNewProfile = _isNewProfile;
            if (isNewProfile)
                CreateProfile();
            else
                LoadProfile();

            SelectedVisibility = CurrentProfile.LauncherVisibility switch
            {
                LauncherVisibility.KeepOpen => UIResource.KeepLauncherOpen,
                LauncherVisibility.Close => UIResource.HideLauncher,
                LauncherVisibility.Hide => UIResource.CloseLauncher,
                _ => throw new ArgumentOutOfRangeException()
            };

            await SaveVersionsToCache();
            ShowSelectedVersions();

            Title = isNewProfile ? UIResource.NewProfileTitle : UIResource.EditProfileTitle;

            CurrentProfile.SelectedVersionsChanged += (sender, args) => { ShowSelectedVersions(); };

            Save = new RelayCommand(() =>
            {
                if (isNewProfile)
                    SaveNewProfile();
                else
                    SaveEditedProfile();

                Messenger.Default.Send(new ProfilesChangedMessage());
            });
            OpenDirectory = new RelayCommand(() =>
            {
                _settingsModel.OpenGameDirectory(CurrentProfile.GameDirectory);
            });
        }

        private void ShowSelectedVersions()
        {
            Versions.Clear();
            if (CurrentProfile.ShowAlpha)
            {
                foreach (var version in _versionsCache.Alpha)
                    Versions.Add(version);
            }

            if (CurrentProfile.ShowBeta)
            {
                foreach (var version in _versionsCache.Beta)
                    Versions.Add(version);
            }

            if (CurrentProfile.ShowRelease)
            {
                foreach (var version in _versionsCache.Release)
                    Versions.Add(version);
            }

            if (CurrentProfile.ShowSnapshot)
            {
                foreach (var version in _versionsCache.Snapshot)
                    Versions.Add(version);
            }

            if (CurrentProfile.ShowCustom)
            {
                foreach (var version in _versionsCache.Custom)
                    Versions.Add(version);
            }
        }

        private void SaveNewProfile()
        {
            _settingsModel.SaveProfile(CurrentProfile);
            Messenger.Default.Send(new StatusMessage(UIResource.NewProfileStatus));
        }

        private void SaveEditedProfile()
        {
            _settingsModel.EditProfile(_oldProfileName, CurrentProfile);
            Messenger.Default.Send(new StatusMessage(UIResource.ProfileEditedStatus));
        }

        private async Task SaveVersionsToCache()
        {
            _versionsCache = await _settingsModel.DownloadAllVersions();
        }

        private void CreateProfile()
        {
            CurrentProfile = new Profile
            {
                JavaFile = _settingsModel.FindJava()
            };
        }

        private void LoadProfile()
        {
            CurrentProfile = _settingsModel.LoadLastProfile();

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