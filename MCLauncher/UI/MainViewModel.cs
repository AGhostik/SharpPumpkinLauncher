using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MainModel _mainModel;
        
        public MainViewModel(MainModel mainModel)
        {
            _mainModel = mainModel;
            _init();
        }

        public string CurrentProfileName { get; set; }
        public ObservableCollection<string> Profiles { get; set; } = new ObservableCollection<string>();

        public RelayCommand Start { get; set; }
        public RelayCommand NewProfile { get; set; }
        public RelayCommand EditProfile { get; set; }
        public RelayCommand DeleteProfile { get; set; }

        private void _init()
        {
            foreach (var profile in _mainModel.GetProfiles())
            {
                Profiles.Add(profile);
            }

            CurrentProfileName = _mainModel.GetLastProfile();

            Start = new RelayCommand(() =>
            {
                _mainModel.StartGame();
            });
            NewProfile = new RelayCommand(() =>
            {
                _mainModel.OpenProfileCreatingWindow();
            });
            EditProfile = new RelayCommand(() =>
            {
                _mainModel.OpenProfileEditingWindow();
            });
            DeleteProfile = new RelayCommand(() =>
            {
                _mainModel.DeleteProfile(CurrentProfileName);
            });
        }
    }
}