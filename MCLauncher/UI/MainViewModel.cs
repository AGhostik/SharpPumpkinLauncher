using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MCLauncher.UI
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
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
                var settingsViewModel = new SettingsViewModel();
                var window = new SettingsView(settingsViewModel)
                {
                    Owner = Application.Current.MainWindow,
                    Title = UIResource.NewProfileTitle
                };
                window.Show();
                //
            });
            EditProfile = new RelayCommand(() =>
            {
                //
            });
            DeleteProfile = new RelayCommand(() =>
            {
                //
            });
        }
    }
}