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
    
    private SettingsData _currentSettings;
    private ProfileViewModel? _selectedProfile;
    private object? _mainContent;
    private bool _dontSaveSelectedProfile;
    private bool _isVersionsLoaded;
    private bool _isProfilesComboboxEnabled;
    private bool _isGameStarted;

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
        NewProfileCommand = ReactiveCommand.Create(NewProfile, CanCreateNewProfile);
        EditProfileCommand = ReactiveCommand.Create(EditProfile, CanEditProfile);
        DeleteProfileCommand = ReactiveCommand.Create(DeleteProfile, CanDeleteProfile);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings, CanOpenSettings);
        
        CanOpenSettings.OnNext(true);
        
        _currentSettings = SetupFromSettings();
        settingsViewModel.Directory = _currentSettings.Directory;
        settingsViewModel.DefaultPlayerName = _currentSettings.DefaultPlayerName;
        settingsViewModel.LauncherVisibility = _currentSettings.LauncherVisibility;
        
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

    private async void StartGame()
    {
        if (SelectedProfile == null || string.IsNullOrEmpty(_currentSettings.Directory))
            return;

        _isGameStarted = true;
        
        UpdateCanCreateProfile();
        UpdateCanEditProfile();
        UpdateCanDeleteProfile();
        UpdateProfilesComboboxEnabled();
        UpdateCanOpenSettings();
        
        MainContent = _progressControl;
        _mainWindowModel.StartGameProgress += OnStartGameProgress;
        
        await _mainWindowModel.StartGame(SelectedProfile, _currentSettings.Directory,
            () => Dispatcher.UIThread.InvokeAsync(GameExited));
        
        _mainWindowModel.StartGameProgress -= OnStartGameProgress;
    }

    private void GameExited()
    {
        _isGameStarted = false;

        _progressViewModel.Text = string.Empty;
        _progressViewModel.ProgressValue = 0;

        UpdateCanCreateProfile();
        UpdateCanEditProfile();
        UpdateCanDeleteProfile();
        UpdateProfilesComboboxEnabled();
        UpdateCanOpenSettings();
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
        MainContent = _settingsControl;
    }

    private void SettingsSaved(SettingsData settingsData)
    {
        _currentSettings = settingsData;
        _mainWindowModel.SaveSettings(settingsData);
    }
    
    private void OnVersionsLoaded(Versions versions)
    {
        _isVersionsLoaded = true;

        UpdateCanCreateProfile();
        UpdateCanEditProfile();
        UpdateCanDeleteProfile();
        UpdateProfilesComboboxEnabled();
    }
    
    private void UpdateCanStartGame()
    {
        var canStartGame = SelectedProfile?.SelectedVersion != null &&
                           !string.IsNullOrEmpty(SelectedProfile.PlayerName) &&
                           !string.IsNullOrEmpty(SelectedProfile.SelectedVersion.Id) &&
                           !string.IsNullOrEmpty(_currentSettings.Directory) &&
                           _isVersionsLoaded;
        
        CanStartGame.OnNext(canStartGame);
    }

    private void NewProfile()
    {
        var profileViewModel = ProfileViewModel.CreateNew(
            Profiles.Select(profile => profile.ProfileName),
            NewProfileSaved,
            SetDefaultMainContent);

        profileViewModel.PlayerName = _currentSettings.DefaultPlayerName;
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

    private void UpdateProfilesComboboxEnabled()
    {
        IsProfilesComboboxEnabled = Profiles.Count > 0 && !_isGameStarted;
    }

    private void UpdateCanOpenSettings()
    {
        CanOpenSettings.OnNext(!_isGameStarted);
    }

    private SettingsData SetupFromSettings()
    {
        var settingsLoaded = _mainWindowModel.LoadSettings(out var profiles, out var lastSelectedProfile,
            out var settingsData);

        if (settingsLoaded && settingsData != null)
        {
            Profiles.AddRange(profiles);
            _dontSaveSelectedProfile = true;
            SelectedProfile = lastSelectedProfile;
            _dontSaveSelectedProfile = false;

            return settingsData;
        }

        CreateDefaultSettings(out var defaultSettingsData);
        _mainWindowModel.SaveSettings(defaultSettingsData);

        return defaultSettingsData;
    }

    private void CreateDefaultSettings(out SettingsData settingsData)
    {
        settingsData = new SettingsData();
        var profileViewModel = ProfileViewModel.CreateDefault(NewProfileSaved);
        profileViewModel.PlayerName = settingsData.DefaultPlayerName;
        _mainWindowModel.VersionsLoaded += profileViewModel.SetVersions;
    }
}