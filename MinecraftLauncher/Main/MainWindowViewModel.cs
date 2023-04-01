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
using MinecraftLauncher.Main.Settings;
using ReactiveUI;

namespace MinecraftLauncher.Main;

public sealed class MainWindowViewModel : ReactiveObject
{
    private readonly MainWindowModel _mainWindowModel;
    private readonly ProgressViewModel _progressViewModel;
    private readonly ProgressControl _progressControl;
    private readonly SettingsControl _settingsControl;
    
    private readonly bool _dontSaveSelectedProfile;
    
    private ProfileViewModel? _selectedProfile;
    private object? _mainContent;
    private bool _isVersionsLoaded;
    private bool _isGameStarted;
    private bool _isProfilesComboboxEnabled;
    private bool _isStartGameVisible;

    public MainWindowViewModel(MainWindowModel mainWindowModel)
    {
        _mainWindowModel = mainWindowModel;

        mainWindowModel.VersionsLoaded += OnVersionsLoaded;
        mainWindowModel.AllProfilesLoaded += UpdateCanStartGame;
        
        _progressViewModel = new ProgressViewModel();
        _progressControl = new ProgressControl() { DataContext = _progressViewModel };

        var settingsViewModel = new SettingsViewModel(SettingsSaved, SetDefaultMainContent);
        _settingsControl = new SettingsControl() { DataContext = settingsViewModel };
        
        StartGameCommand = ReactiveCommand.Create(StartGame, CanStartGame);
        AbortGameCommand = ReactiveCommand.Create(AbortStartGame, CanAbortGame);
        NewProfileCommand = ReactiveCommand.Create(NewProfile, CanCreateNewProfile);
        EditProfileCommand = ReactiveCommand.Create(EditProfile, CanEditProfile);
        DeleteProfileCommand = ReactiveCommand.Create(DeleteProfile, CanDeleteProfile);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings, CanOpenSettings);
        
        CanOpenSettings.OnNext(true);
        IsStartGameVisible = true;
        
        settingsViewModel.Directory = _mainWindowModel.CurrentSettings.Directory;
        settingsViewModel.DefaultPlayerName = _mainWindowModel.CurrentSettings.DefaultPlayerName;
        settingsViewModel.LauncherVisibility = _mainWindowModel.CurrentSettings.LauncherVisibility;
        
        Profiles.AddRange(_mainWindowModel.Profiles);
        _dontSaveSelectedProfile = true;
        SelectedProfile = _mainWindowModel.LastSelectedProfile;
        _dontSaveSelectedProfile = false;
        UpdateProfilesComboboxEnabled();
        
        SetDefaultMainContent();
    }

    public ProfileViewModel? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProfile, value);

            if (!_dontSaveSelectedProfile)
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

    public bool IsProfilesComboboxEnabled
    {
        get => _isProfilesComboboxEnabled;
        set => this.RaiseAndSetIfChanged(ref _isProfilesComboboxEnabled, value);
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
    
    public ReactiveCommand<Unit, Unit> AbortGameCommand { get; }
    
    private Subject<bool> CanAbortGame { get; } = new();

    public bool IsStartGameVisible
    {
        get => _isStartGameVisible;
        set => this.RaiseAndSetIfChanged(ref _isStartGameVisible, value);
    }

    private bool IsVersionsLoaded
    {
        get => _isVersionsLoaded;
        set
        {
            _isVersionsLoaded = value;
            UpdateCanStartGame();
            UpdateCanCreateProfile();
            UpdateCanEditProfile();
            UpdateCanDeleteProfile();
        }
    }

    private bool IsGameStarted
    {
        get => _isGameStarted;
        set
        {
            _isGameStarted = value;
            UpdateCanStartGame();
            UpdateCanCreateProfile();
            UpdateCanEditProfile();
            UpdateCanDeleteProfile();
            UpdateProfilesComboboxEnabled();
            UpdateCanOpenSettings();
            UpdateCanAbortGame();
        }
    }

    private async void StartGame()
    {
        if (SelectedProfile == null || IsGameStarted)
            return;

        IsGameStarted = true;
        IsStartGameVisible = false;
        
        MainContent = _progressControl;
        _mainWindowModel.StartGameProgress += OnStartGameProgress;
        
        await _mainWindowModel.StartGame(SelectedProfile, () => Dispatcher.UIThread.InvokeAsync(GameExited));
        
        _mainWindowModel.StartGameProgress -= OnStartGameProgress;
    }

    private void AbortStartGame()
    {
        if (!IsGameStarted)
            return;
        
        _mainWindowModel.AbortStartGame();
        GameExited();
    }

    private void GameExited()
    {
        IsGameStarted = false;
        IsStartGameVisible = true;

        //_progressViewModel.Text = string.Empty;
        _progressViewModel.ProgressValue = 0;
    }
    
    private void OnStartGameProgress(LaunchProgress status, float progress01)
    {
        //todo: 
        _progressViewModel.Text = status switch
        {
            LaunchProgress.GetVersionData => "GetVersionData",
            LaunchProgress.DownloadFiles => "DownloadFiles",
            LaunchProgress.StartGame => "StartGame",
            LaunchProgress.GameAborted => "GameAborted",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
            
        _progressViewModel.ProgressValue = 100f * progress01;
    }

    private void OpenSettings()
    {
        MainContent = _settingsControl;
    }

    private void SettingsSaved(SettingsData settingsData)
    {
        _mainWindowModel.SetSettingsData(settingsData);
        UpdateCanStartGame();
    }
    
    private void OnVersionsLoaded(Versions versions)
    {
        IsVersionsLoaded = true;
    }

    private void NewProfile()
    {
        var profileViewModel = ProfileViewModel.CreateNew(
            Profiles.Select(profile => profile.ProfileName),
            NewProfileSaved,
            SetDefaultMainContent);

        profileViewModel.PlayerName = _mainWindowModel.CurrentSettings.DefaultPlayerName;
        _mainWindowModel.VersionsLoaded += profileViewModel.SetVersions;
        
        MainContent = new ProfileControl()
        {
            DataContext = profileViewModel
        };
    }

    private void NewProfileSaved(ProfileViewModel newProfileViewModel)
    {
        Profiles.Add(newProfileViewModel);
        SelectedProfile ??= newProfileViewModel;

        SetDefaultMainContent();
        UpdateProfilesComboboxEnabled();
        _mainWindowModel.SaveProfile(newProfileViewModel);
    }
    
    private void EditProfile()
    {
        if (SelectedProfile == null)
            return;
        
        MainContent = new ProfileControl()
        {
            DataContext = ProfileViewModel.Edit(
                SelectedProfile,
                Profiles.Where(profile => profile.ProfileName != SelectedProfile.ProfileName)
                    .Select(profile => profile.ProfileName),
                ProfileEdited,
                SetDefaultMainContent)
        };
    }

    private void ProfileEdited(ProfileViewModel editedProfileViewModel)
    {
        SetDefaultMainContent();
    }
    
    private void DeleteProfile()
    {
        if (SelectedProfile != null)
            Profiles.Remove(SelectedProfile);
        
        IsProfilesComboboxEnabled = Profiles.Count > 0;
    }
    
    private void SetDefaultMainContent()
    {
        MainContent = _progressControl;
    }
    
    private void UpdateCanStartGame()
    {
        var canStartGame = 
            !IsGameStarted &&
            SelectedProfile?.SelectedVersion != null &&
            !string.IsNullOrEmpty(SelectedProfile.PlayerName) &&
            !string.IsNullOrEmpty(SelectedProfile.SelectedVersion.Id) &&
            !string.IsNullOrEmpty(_mainWindowModel.CurrentSettings.Directory) &&
            IsVersionsLoaded;
        
        CanStartGame.OnNext(canStartGame);
    }

    private void UpdateCanCreateProfile()
    {
        CanCreateNewProfile.OnNext(IsVersionsLoaded && !IsGameStarted);
    }
    
    private void UpdateCanEditProfile()
    {
        CanEditProfile.OnNext(IsVersionsLoaded && SelectedProfile != null && !IsGameStarted);
    }
    
    private void UpdateCanDeleteProfile()
    {
        CanDeleteProfile.OnNext(IsVersionsLoaded && SelectedProfile != null && !IsGameStarted);
    }

    private void UpdateProfilesComboboxEnabled()
    {
        IsProfilesComboboxEnabled = Profiles.Count > 0 && !IsGameStarted;
    }

    private void UpdateCanOpenSettings()
    {
        CanOpenSettings.OnNext(!IsGameStarted);
    }

    private void UpdateCanAbortGame()
    {
        CanAbortGame.OnNext(IsGameStarted);
    }
}