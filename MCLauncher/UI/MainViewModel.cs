using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MainModel _mainModel;
        private string _currentProfileName;
        private bool _isEditActive;
        private bool _isStartActive;

        public MainViewModel(MainModel mainModel)
        {
            _mainModel = mainModel;
            _init();
        }

        public bool IsEditActive
        {
            get => _isEditActive;
            set => Set(ref _isEditActive, value);
        }

        public string CurrentProfileName
        {
            get => _currentProfileName;
            set
            {
                Set(ref _currentProfileName, value);

                if (!string.IsNullOrEmpty(value))
                {
                    IsEditActive = true;
                    IsStartActive = true;
                    _mainModel.SaveLastProfileName(value);
                }
                else
                {
                    IsEditActive = false;
                    IsStartActive = false;
                }
            }
        }

        public ObservableCollection<string> Profiles { get; set; } = new ObservableCollection<string>();

        public RelayCommand Start { get; set; }
        public RelayCommand NewProfile { get; set; }
        public RelayCommand EditProfile { get; set; }
        public RelayCommand DeleteProfile { get; set; }

        public bool IsStartActive { get => _isStartActive; set => Set(ref _isStartActive, value); }

        private void _init()
        {
            IsStartActive = false;
            IsEditActive = false;
            _refreshProfiles();

            CurrentProfileName = _mainModel.GetLastProfile();

            Start = new RelayCommand(async () => { await _mainModel.StartGame(); });
            NewProfile = new RelayCommand(() => { _mainModel.OpenProfileCreatingWindow(); });
            EditProfile = new RelayCommand(() => { _mainModel.OpenProfileEditingWindow(); });
            DeleteProfile = new RelayCommand(() => { _mainModel.DeleteProfile(CurrentProfileName); });

            Messenger.Default.Register(this, (object dummy) => { _refreshProfiles(); });
        }

        private void _refreshProfiles()
        {
            Profiles.Clear();
            foreach (var profile in _mainModel.GetProfiles()) Profiles.Add(profile);
            CurrentProfileName = _mainModel.GetLastProfile();
        }
    }
}