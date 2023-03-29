using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Threading;
using DynamicData;
using Launcher.PublicData;
using MinecraftLauncher.Main.Profile;
using MinecraftLauncher.Main.Progress;
using ReactiveUI;

namespace MinecraftLauncher.Main;

public sealed class MainWindowViewModel : ReactiveObject
{
    private readonly MainWindowModel _mainWindowModel;
    private readonly ProgressViewModel _progressViewModel;
    private readonly ProgressControl _progressControl;
    
    private ProfileViewModel? _selectedProfile;
    private object? _mainContent;
    private bool _isVersionsLoaded;
    private bool _isVersionsComboboxEnabled;
    private bool _skipSelectedProfileSaving;
    private bool _isGameStarted;

    public MainWindowViewModel(MainWindowModel mainWindowModel)
    {
        _mainWindowModel = mainWindowModel;

        mainWindowModel.VersionsLoaded += MainWindowModelOnVersionsLoaded;
        
        _progressViewModel = new ProgressViewModel();
        _progressControl = new ProgressControl() { DataContext = _progressViewModel };
        
        StartGameCommand = ReactiveCommand.Create(StartGame, CanStartGame);
        NewProfileCommand = ReactiveCommand.Create(NewProfile, CanCreateNewProfile);
        EditProfileCommand = ReactiveCommand.Create(EditProfile, CanEditProfile);
        DeleteProfileCommand = ReactiveCommand.Create(DeleteProfile, CanDeleteProfile);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings, CanOpenSettings);
    }

    public ProfileViewModel? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProfile, value);

            if (!_skipSelectedProfileSaving)
            {
                if (value != null && !string.IsNullOrEmpty(value.ProfileName))
                    _mainWindowModel.SaveSelectedProfile(value.ProfileName);
            }

            UpdateCanEditProfile();
            UpdateCanDeleteProfile();
            UpdateCanStartGame();
        }
    }

    public ObservableCollection<ProfileViewModel> Profiles { get; } = new();

    public bool IsVersionsComboboxEnabled
    {
        get => _isVersionsComboboxEnabled;
        set => this.RaiseAndSetIfChanged(ref _isVersionsComboboxEnabled, value);
    }

    public object? MainContent
    {
        get => _mainContent;
        set => this.RaiseAndSetIfChanged(ref _mainContent, value);
    }

    public ReactiveCommand<Unit, Unit> NewProfileCommand { get; }
    
    private Subject<bool> CanCreateNewProfile { get; } = new();

    public ReactiveCommand<Unit, Unit> EditProfileCommand { get; }
    
    private Subject<bool> CanEditProfile { get; } = new();

    public ReactiveCommand<Unit, Unit> DeleteProfileCommand { get; }
    
    private Subject<bool> CanDeleteProfile { get; } = new();

    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }
    
    private Subject<bool> CanOpenSettings { get; } = new();
    
    public ReactiveCommand<Unit, Unit> StartGameCommand { get; }
    
    private Subject<bool> CanStartGame { get; } = new();

    private async void StartGame()
    {
        if (SelectedProfile == null)
            return;

        _isGameStarted = true;
        
        UpdateCanCreateProfile();
        UpdateCanEditProfile();
        UpdateCanDeleteProfile();
        UpdateVersionsComboboxEnabled();
        
        MainContent = _progressControl;
        _mainWindowModel.StartGameProgress += OnStartGameProgress;
        
        await _mainWindowModel.StartGame(SelectedProfile, () => Dispatcher.UIThread.InvokeAsync(GameExited));
        
        _mainWindowModel.StartGameProgress -= OnStartGameProgress;
    }

    private void GameExited()
    {
        _isGameStarted = false;

        _progressViewModel.Text = string.Empty;
        _progressViewModel.ProgressValue = 0;
        MainContent = null;
        
        UpdateCanCreateProfile();
        UpdateCanEditProfile();
        UpdateCanDeleteProfile();
        UpdateVersionsComboboxEnabled();
    }
    
    private void OnStartGameProgress(LaunchProgress status, float progress01)
    {
        //todo: 
        _progressViewModel.Text = status switch
        {
            LaunchProgress.GetVersionData => "GetVersionData",
            LaunchProgress.GetFileList => "GetFileList",
            LaunchProgress.GetLaunchArguments => "GetLaunchArguments",
            LaunchProgress.DownloadFiles => "DownloadFiles",
            LaunchProgress.StartGame => "StartGame",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
            
        _progressViewModel.ProgressValue = 100f * progress01;
    }

    private void OpenSettings()
    {
        //
    }
    
    private void MainWindowModelOnVersionsLoaded()
    {
        _isVersionsLoaded = true;

        Profiles.AddRange(_mainWindowModel.GetProfiles());
        
        _skipSelectedProfileSaving = true;
        var lastSelectedProfileName = _mainWindowModel.GetLastSelectedProfile();
        var selectedProfile = Profiles.FirstOrDefault(profile => profile.ProfileName == lastSelectedProfileName);
        SelectedProfile = selectedProfile;
        _skipSelectedProfileSaving = false;
        
        UpdateCanCreateProfile();
        UpdateVersionsComboboxEnabled();
    }
    
    private void UpdateCanStartGame()
    {
        var canStartGame = SelectedProfile != null && SelectedProfile.SelectedVersion != null &&
                           !string.IsNullOrEmpty(SelectedProfile.PlayerName) &&
                           !string.IsNullOrEmpty(SelectedProfile.Directory) &&
                           !string.IsNullOrEmpty(SelectedProfile.SelectedVersion.Id) &&
                           _isVersionsLoaded;
        
        CanStartGame.OnNext(canStartGame);
    }

    private void NewProfile()
    {
        if (_mainWindowModel.AvailableVersions == null)
            return;
        
        MainContent = new ProfileControl()
        {
            DataContext = ProfileViewModel.CreateNew(_mainWindowModel.AvailableVersions,
                Profiles.Select(profile => profile.ProfileName),
                NewProfileSaved, CloseProfileContent)
        };
    }

    private void NewProfileSaved(ProfileViewModel newProfileViewModel)
    {
        Profiles.Add(newProfileViewModel);
        SelectedProfile ??= newProfileViewModel;

        MainContent = null;
        UpdateVersionsComboboxEnabled();
        _mainWindowModel.SaveProfile(newProfileViewModel);
    }
    
    private void EditProfile()
    {
        if (SelectedProfile == null || _mainWindowModel.AvailableVersions == null)
            return;
        
        MainContent = new ProfileControl()
        {
            DataContext = ProfileViewModel.Edit(SelectedProfile, _mainWindowModel.AvailableVersions,
                Profiles.Where(profile => profile.ProfileName != SelectedProfile.ProfileName).Select(profile => profile.ProfileName),
                ProfileEdited, CloseProfileContent)
        };
    }

    private void ProfileEdited(ProfileViewModel editedProfileViewModel)
    {
        MainContent = null;
    }
    
    private void DeleteProfile()
    {
        if (SelectedProfile != null)
            Profiles.Remove(SelectedProfile);
        
        IsVersionsComboboxEnabled = Profiles.Count > 0;
    }
    
    private void CloseProfileContent()
    {
        MainContent = null;
    }

    private void UpdateCanCreateProfile()
    {
        CanCreateNewProfile.OnNext(_isVersionsLoaded && !_isGameStarted);
    }
    
    private void UpdateCanEditProfile()
    {
        CanEditProfile.OnNext(_isVersionsLoaded && SelectedProfile != null && !_isGameStarted);
    }
    
    private void UpdateCanDeleteProfile()
    {
        CanDeleteProfile.OnNext(_isVersionsLoaded && SelectedProfile != null && !_isGameStarted);
    }

    private void UpdateVersionsComboboxEnabled()
    {
        IsVersionsComboboxEnabled = Profiles.Count > 0 && !_isGameStarted;
    }
}