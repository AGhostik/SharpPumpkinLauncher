using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MCLauncher.LauncherWindow;

public sealed class LauncherViewModel : ObservableObject
{
    private readonly LauncherModel _launcherModel;
    private ProfileViewModel? _currentProfile;
    
    private Visibility _progresBarVisibility;
    private float _progress;
    private string? _status;
    
    private bool _canAddProfile;
    private bool _canEditProfile;
    private bool _canRemoveProfile;
    private bool _canStartGame;

    public LauncherViewModel(LauncherModel launcherModel)
    {
        _launcherModel = launcherModel;
        
        _canRemoveProfile = false;
        _canEditProfile = false;

        _progress = 0;
        _progresBarVisibility = Visibility.Collapsed;

        Start = new AsyncRelayCommand(StartGame);
        NewProfile = new RelayCommand(_launcherModel.CreateProfile);
        EditProfile = new RelayCommand<ProfileViewModel?>(_launcherModel.EditProfile);
        DeleteProfile = new RelayCommand<ProfileViewModel?>(_launcherModel.DeleteProfile);

        Status = "Loading data";
        
        launcherModel.ProfileAdded += LauncherModelOnProfileAdded;
        launcherModel.LaunchProgress += LauncherModelOnLaunchProgress;
        launcherModel.MinecraftDataLoaded += LauncherModelOnMinecraftDataLoaded;

        RefreshProfiles();
    }

    private async Task StartGame()
    {
        if (CurrentProfile == null)
            return;

        CanStartGame = false;
        
        await _launcherModel.StartGame(CurrentProfile, () =>
        {
            Status = "Game exited";
            Progress = 0;
            CanStartGame = true;
        });
    }

    private void LauncherModelOnProfileAdded(ProfileViewModel profile)
    {
        Profiles.Add(profile);

        if (CurrentProfile == null)
            CurrentProfile = profile;
    }

    private void LauncherModelOnMinecraftDataLoaded()
    {
        CanAddProfile = true;
        Status = "Ready";
    }

    private void LauncherModelOnLaunchProgress(string status, float progress01)
    {
        Status = status;

        if (progress01 <= 0f)
        {
            ProgresBarVisibility = Visibility.Collapsed;
            Progress = 0f;
        }
        else
        {
            ProgresBarVisibility = Visibility.Visible;
            Progress = progress01 * 100f;
        }
    }

    public ProfileViewModel? CurrentProfile
    {
        get => _currentProfile;
        set
        {
            SetProperty(ref _currentProfile, value);
            
            if (value != null)
                _launcherModel.SaveLastProfile(value);
            
            CanEditProfile = _currentProfile != null;
            CanRemoveProfile = _currentProfile != null;
            CanStartGame = _currentProfile != null;
        }
    }
        
    public ObservableCollection<ProfileViewModel> Profiles { get; } = new();

    public ICommand? Start { get; }
    public ICommand? NewProfile { get; }
    public ICommand? EditProfile { get; }
    public ICommand? DeleteProfile { get; }

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

    public bool CanAddProfile
    {
        get => _canAddProfile;
        set => SetProperty(ref _canAddProfile, value);
    }

    public bool CanEditProfile
    {
        get => _canEditProfile;
        set => SetProperty(ref _canEditProfile, value);
    }

    public bool CanRemoveProfile
    {
        get => _canRemoveProfile;
        set => SetProperty(ref _canRemoveProfile, value);
    }

    public bool CanStartGame
    {
        get => _canStartGame;
        set => SetProperty(ref _canStartGame, value);
    }

    public Visibility ProgresBarVisibility
    {
        get => _progresBarVisibility;
        set => SetProperty(ref _progresBarVisibility, value);
    }

    private void RefreshProfiles()
    {
        Profiles.Clear();
        var profiles = _launcherModel.GetProfiles();

        for (var i = 0; i < profiles.Count; i++)
            Profiles.Add(profiles[i]);

        CurrentProfile = _launcherModel.GetLastProfile();
        if (CurrentProfile == null && profiles.Count > 0)
            CurrentProfile = profiles[0];
    }
}