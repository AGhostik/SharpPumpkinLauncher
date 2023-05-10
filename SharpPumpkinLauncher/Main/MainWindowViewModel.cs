using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Threading;
using DynamicData;
using Launcher.PublicData;
using ReactiveUI;
using SharpPumpkinLauncher.Main.About;
using SharpPumpkinLauncher.Main.Profile;
using SharpPumpkinLauncher.Main.Progress;
using SharpPumpkinLauncher.Main.Settings;
using SharpPumpkinLauncher.Main.Validation;

namespace SharpPumpkinLauncher.Main;

public sealed class MainWindowViewModel : ReactiveObject
{
    private readonly MainWindowModel _mainWindowModel;
    private readonly ProgressControl _progressControl;
    private readonly SettingsControl _settingsControl;
    private readonly AboutControl _aboutControl;
    private readonly VersionsLoader _versionsLoader;
    
    private ProfileViewModel? _selectedProfile;
    private object? _mainContent;
    private bool _isVersionsLoaded;
    private bool _isGameStarted;
    private bool _isProfilesComboboxEnabled;
    private bool _isStartGameVisible;

    public MainWindowViewModel()
    {
        _versionsLoader = ServiceProvider.VersionsLoader;
        _mainWindowModel = new MainWindowModel(ServiceProvider.MinecraftLauncher, ServiceProvider.SettingsManager);

        var progressViewModel = new ProgressViewModel(_mainWindowModel);
        _progressControl = new ProgressControl() { DataContext = progressViewModel };

        var settingsViewModel = new SettingsViewModel(SettingsSaved, SetDefaultMainContent);
        _settingsControl = new SettingsControl() { DataContext = settingsViewModel };

        var aboutViewModel = new AboutViewModel(SetDefaultMainContent);
        _aboutControl = new AboutControl() { DataContext = aboutViewModel };
        
        StartGameCommand = ReactiveCommand.Create(StartGame, CanStartGame);
        AbortGameCommand = ReactiveCommand.Create(AbortStartGame, CanAbortGame);
        NewProfileCommand = ReactiveCommand.Create(NewProfile, CanCreateNewProfile);
        EditProfileCommand = ReactiveCommand.Create(EditProfile, CanEditProfile);
        DeleteProfileCommand = ReactiveCommand.Create(DeleteProfile, CanDeleteProfile);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings, CanOpenSettings);
        OpenAboutCommand = ReactiveCommand.Create(OpenAbout, CanOpenAbout);
        
        CanOpenSettings.OnNext(true);
        CanOpenAbout.OnNext(true);
        IsStartGameVisible = true;
        
        settingsViewModel.SetUp(_mainWindowModel.CurrentSettings);

        if (_mainWindowModel.LastSelectedProfile == null && _mainWindowModel.Profiles.Count == 0)
        {
            _versionsLoader.VersionsLoaded += SetDefaultProfile;
        }
        else
        {
            Profiles.AddRange(_mainWindowModel.Profiles);
            SelectedProfile = _mainWindowModel.LastSelectedProfile;
        }
        
        _versionsLoader.VersionsLoaded += SetIsVersionsLoaded;

        UpdateProfilesComboboxEnabled();
        SetDefaultMainContent();
    }

    public ProfileViewModel? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProfile, value);

            if (!string.IsNullOrEmpty(value?.ProfileName))
                MainWindowModel.SaveSelectedProfile(value.ProfileName);

            UpdateCanEditProfile();
            UpdateCanDeleteProfile();
            UpdateCanStartGame();
            SetDefaultMainContent();
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
    
    public ReactiveCommand<Unit, Unit> OpenAboutCommand { get; }
    
    private Subject<bool> CanOpenAbout { get; } = new();

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
            UpdateCanOpenAbout();
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
        
        await _mainWindowModel.StartGame(SelectedProfile, () => Dispatcher.UIThread.InvokeAsync(GameExited));
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
    }

    private void OpenSettings()
    {
        MainContent = _settingsControl;
    }

    private void SettingsSaved()
    {
        UpdateCanStartGame();
        SetDefaultMainContent();
    }

    private void OpenAbout()
    {
        MainContent = _aboutControl;
    }
    
    private void SetIsVersionsLoaded(Versions versions)
    {
        if (versions.IsEmpty)
            return;
        
        IsVersionsLoaded = true;
        _versionsLoader.VersionsLoaded -= SetIsVersionsLoaded;
    }
    
    private void SetDefaultProfile(Versions versions)
    {
        if (versions.Latest == null)
            return;
        
        _versionsLoader.VersionsLoaded -= SetDefaultProfile;
        
        var profile = ProfileViewModel.Create(versions.Latest, _mainWindowModel.CurrentSettings.DefaultPlayerName);
        Profiles.Add(profile);
        SelectedProfile = profile;

        UpdateProfilesComboboxEnabled();
    }

    private void NewProfile()
    {
        var restrictedNames = Profiles.Select(profile => profile.ProfileName);
        var profileViewModel = ProfileViewModel.CreateNew(_mainWindowModel.CurrentSettings.DefaultPlayerName,
            restrictedNames, NewProfileSaved, SetDefaultMainContent);

        MainContent = new ProfileControl()
        {
            DataContext = profileViewModel
        };
    }

    private void NewProfileSaved(ProfileViewModel newProfileViewModel)
    {
        Profiles.Add(newProfileViewModel);
        SelectedProfile = newProfileViewModel;

        SetDefaultMainContent();
        UpdateProfilesComboboxEnabled();
    }
    
    private void EditProfile()
    {
        if (SelectedProfile == null)
            return;

        var restrictedNames = Profiles.Where(profile => profile.ProfileName != SelectedProfile.ProfileName)
            .Select(profile => profile.ProfileName);
        
        var profileViewModel =
            ProfileViewModel.Edit(SelectedProfile, restrictedNames, EditProfileSaved, SetDefaultMainContent);
        
        MainContent = new ProfileControl()
        {
            DataContext = profileViewModel
        };
    }

    private void EditProfileSaved(ProfileViewModel profileViewModel)
    {
        SelectedProfile = null;
        SelectedProfile = profileViewModel;
        
        SetDefaultMainContent();
    }

    private void DeleteProfile()
    {
        if (SelectedProfile == null)
            return;
        
        SelectedProfile.OnDelete();
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
            IsVersionsLoaded &&
            SelectedProfile?.SelectedVersion != null &&
            !string.IsNullOrEmpty(SelectedProfile.SelectedVersion.Id) &&
            PlayerNameValidation.IsPlayerNameValid(SelectedProfile.PlayerName) &&
            DirectoryValidation.IsDirectoryValid(_mainWindowModel.CurrentSettings.Directory);
        
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
    
    private void UpdateCanOpenAbout()
    {
        CanOpenAbout.OnNext(!IsGameStarted);
    }
    
    private void UpdateCanAbortGame()
    {
        CanAbortGame.OnNext(IsGameStarted);
    }
}