using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.Messages;

namespace MCLauncher.LauncherWindow;

public class LauncherViewModel : ObservableObject
{
    private readonly ILauncherModel _launcherModel;
    private string? _currentProfileName;
    private bool _isEditActive;
    private bool _isStartActive;
    private Visibility _progresBarVisibility;
    private float _progress;
    private string? _status;

    public LauncherViewModel(ILauncherModel launcherModel)
    {
        _launcherModel = launcherModel;
        Init();
    }

    public bool IsEditActive
    {
        get => _isEditActive;
        set => SetProperty(ref _isEditActive, value);
    }

    public string? CurrentProfileName
    {
        get => _currentProfileName;
        set
        {
            SetProperty(ref _currentProfileName, value);

            if (!string.IsNullOrEmpty(value))
            {
                IsEditActive = true;
                IsStartActive = true;
                _launcherModel.SaveLastProfileName(value);
            }
            else
            {
                IsEditActive = false;
                IsStartActive = false;
            }
        }
    }
        
    public ObservableCollection<string?> Profiles { get; } = new();

    public ICommand? Start { get; set; }
    public ICommand? NewProfile { get; set; }
    public ICommand? EditProfile { get; set; }
    public ICommand? DeleteProfile { get; set; }

    public string? Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public float Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public bool IsStartActive
    {
        get => _isStartActive;
        set => SetProperty(ref _isStartActive, value);
    }

    public Visibility ProgresBarVisibility
    {
        get => _progresBarVisibility;
        set => SetProperty(ref _progresBarVisibility, value);
    }

    private void Init()
    {
        IsStartActive = false;
        IsEditActive = false;
        RefreshProfiles();

        Progress = 0;
        ProgresBarVisibility = Visibility.Collapsed;

        CurrentProfileName = _launcherModel.GetLastProfile();

        Start = new AsyncRelayCommand(async () => { await _launcherModel.StartGame(); });
        NewProfile = new RelayCommand(() => { _launcherModel.OpenNewProfileWindow(); });
        EditProfile = new RelayCommand(() => { _launcherModel.OpenEditProfileWindow(); });
        DeleteProfile = new RelayCommand(() => { _launcherModel.DeleteProfile(CurrentProfileName); });

        WeakReferenceMessenger.Default.Register<ProfilesChangedMessage>(this, (_, _) => { RefreshProfiles(); });
        WeakReferenceMessenger.Default.Register<StatusMessage>(this, (_, message) => { Status = message.Status; });
        WeakReferenceMessenger.Default.Register<InstallProgressMessage>(this, (_, message) =>
        {
            ProgresBarVisibility = message.Percentage < 100 ? Visibility.Visible : Visibility.Collapsed;
            Progress = message.Percentage;
        });
    }

    private void RefreshProfiles()
    {
        Profiles.Clear();
        foreach (var profile in _launcherModel.GetProfiles())
            Profiles.Add(profile);
        CurrentProfileName = _launcherModel.GetLastProfile();
    }
}