using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsModel _settingsModel;
        private string _selectedVisibility;

        public SettingsViewModel(SettingsModel model)
        {
            _settingsModel = model;
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

        private void _init()
        {
            _loadProfile();
            _fillVersions();

            Save = new RelayCommand(_saveProfile);
            OpenDirectory = new RelayCommand(() => { _settingsModel.OpenGameDirectory(CurrentProfile); });
        }

        private void _fillVersions()
        {
            Versions.Clear();
            var versions = _settingsModel.GetVersions(CurrentProfile);
            foreach (var version in versions) Versions.Add(version);
        }

        private void _saveProfile()
        {
            _settingsModel.SaveLastProfile(CurrentProfile);
        }

        private void _loadProfile()
        {
            CurrentProfile = _settingsModel.LoadLastProfile();

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