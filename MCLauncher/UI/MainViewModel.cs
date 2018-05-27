using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Installer _installer;

        public MainViewModel(Installer installer)
        {
            _installer = installer;
            _init();
        }

        public string CurrentProfile { get; set; }
        public ObservableCollection<string> Profiles { get; set; } = new ObservableCollection<string>();

        public RelayCommand Start { get; set; }
        public RelayCommand NewProfile { get; set; }
        public RelayCommand EditProfile { get; set; }
        public RelayCommand DeleteProfile { get; set; }

        private void _init()
        {
            CurrentProfile = "current";
            Profiles.Add("test");
            Profiles.Add("current");
            Start = new RelayCommand(() =>
            {
                MessageBox.Show("Start");
                //
            });
            NewProfile = new RelayCommand(() =>
            {
                //
                _showSettings(UIResource.NewProfileTitle);
            });
            EditProfile = new RelayCommand(() =>
            {
                //
                _showSettings(UIResource.EditProfileTitle);
            });
            DeleteProfile = new RelayCommand(() =>
            {
                //
            });
        }

        private void _showSettings(string title)
        {
            var settingsViewModel = new SettingsViewModel(new SettingsModel(new FileManager()));
            var settingsWindow = new SettingsView()
            {
                Owner = Application.Current.MainWindow,
                DataContext = settingsViewModel,
                Title = title
            };
            settingsWindow.Show();
        }
    }
}