using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Launcher.PublicData;
using MinecraftLauncher.Main.Profile;
using MinecraftLauncher.Main.Progress;
using MinecraftLauncher.Main.Settings;
using MinecraftLauncher.Main.Validation;
using SimpleLogger;
using UserSettings;
using SettingsData = MinecraftLauncher.Main.Settings.SettingsData;

namespace MinecraftLauncher.Main;
using Launcher;

public sealed class MainWindowModel
{
    private readonly MinecraftLauncher _minecraftLauncher;

    private CancellationTokenSource? _cancellationTokenSource;

    private Versions? _offlineVersions;
    private Versions? _onlineVersions;
    
    private Versions? _allVersions;
    
    private int _profilesToLoadCount;
    private int _currentLoadedProfilesCount;

    private Action<Versions>? _versionsLoaded;

    public event Action<Versions>? VersionsLoaded
    {
        add
        {
            _versionsLoaded += value;
            InvokeVersionsLoaded();
        }
        remove => _versionsLoaded -= value;
    }
    public event Action<ProgressLocalizationKeys, float, string?>? UpdateProgressValues;
    public event Action? AllProfilesLoaded;
    private event Action Init;

    public MainWindowModel()
    {
        _minecraftLauncher = new MinecraftLauncher();
        _minecraftLauncher.LaunchMinecraftProgress += UpdateProgress;

        Init += LoadOfflineVersions;
        Init += LoadOnlineVersions;

        if (LoadSettings(out var allProfiles, out var lastSelectedProfile, out var settingsData))
        {
            Profiles = allProfiles;
            LastSelectedProfile = lastSelectedProfile;
            CurrentSettings = settingsData;
        }
        else
        {
            CreateDefaultSettings(out var profileViewModel, out var defaultSettingsData);
            Profiles = new []{ profileViewModel };
            LastSelectedProfile = profileViewModel;
            CurrentSettings = defaultSettingsData;
            SetSettingsData(CurrentSettings);
        }
        
        Init.Invoke();
        UpdateProgressValues?.Invoke(ProgressLocalizationKeys.Loading, 0, null);
    }

    public IReadOnlyList<ProfileViewModel> Profiles { get; }

    public ProfileViewModel? LastSelectedProfile { get; }

    public SettingsData CurrentSettings { get; private set; }

    public async Task StartGame(ProfileViewModel profileViewModel, Action gameExited)
    {
        if (string.IsNullOrEmpty(profileViewModel.PlayerName) ||
            string.IsNullOrEmpty(CurrentSettings.Directory) ||
            profileViewModel.SelectedVersion == null)
        {
            UpdateProgressValues?.Invoke(ProgressLocalizationKeys.InvalidProfile, 0, null);
            return;
        }
        
        var launchData = new LaunchData(
            profileViewModel.PlayerName,
            profileViewModel.SelectedVersion,
            CurrentSettings.Directory,
            CurrentSettings.UseCustomResolution,
            CurrentSettings.ScreenHeight,
            CurrentSettings.ScreenWidth);

        _cancellationTokenSource = new CancellationTokenSource();
        
        var result =
            await _minecraftLauncher.LaunchMinecraft(launchData, _cancellationTokenSource.Token, gameExited.Invoke);

        switch (result)
        {
            case ErrorCode.NoError:
                break;
            case ErrorCode.GameAborted:
                gameExited.Invoke();
                UpdateProgressValues?.Invoke(ProgressLocalizationKeys.Aborted, 0, null);
                break;
            default:
                gameExited.Invoke();
                UpdateProgressValues?.Invoke(ProgressLocalizationKeys.FailToStartGame, 0, null);
                Logger.Log(result);
                break;
        }
        
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    public void AbortStartGame()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void SetSettingsData(SettingsData settingsData)
    {
        var directoryChanged = CurrentSettings.Directory != settingsData.Directory;
        CurrentSettings = settingsData;

        LauncherSettings.Instance.Data.LauncherVisibility = (int)settingsData.LauncherVisibility;
        LauncherSettings.Instance.Data.GameDirectory = settingsData.Directory;
        LauncherSettings.Instance.Data.DefaultPlayerName = settingsData.DefaultPlayerName;
        LauncherSettings.Instance.Data.UseCustomResolution = settingsData.UseCustomResolution;
        LauncherSettings.Instance.Data.ScreenHeight = settingsData.ScreenHeight;
        LauncherSettings.Instance.Data.ScreenWidth = settingsData.ScreenWidth;
        LauncherSettings.Save();
        
        if (directoryChanged)
            LoadOfflineVersions();
    }

    public void SaveSelectedProfile(string profileName)
    {
        LauncherSettings.Instance.Data.LastProfileName = profileName;
        LauncherSettings.Save();
    }

    private void ProfileLoaded(ProfileViewModel profileViewModel)
    {
        _currentLoadedProfilesCount++;
        if (_currentLoadedProfilesCount == _profilesToLoadCount)
            AllProfilesLoaded?.Invoke();
    }
    
    public void SaveProfile(ProfileViewModel profileViewModel)
    {
        if (LauncherSettings.Instance.Data.Profiles == null)
            LauncherSettings.Instance.Data.Profiles = new List<ProfileData>();
        
        if (LauncherSettings.Instance.Data.Profiles.Find(p => p.Name == profileViewModel.ProfileName) != null)
            return;
        
        var profileData = new ProfileData()
        {
            Name = profileViewModel.ProfileName,
            PlayerNickname = profileViewModel.PlayerName,
            MinecraftVersion = profileViewModel.SelectedVersion?.Id,
            Alpha = profileViewModel.Alpha,
            Beta = profileViewModel.Beta,
            Custom = profileViewModel.Custom,
            Release = profileViewModel.Release,
            Snapshot = profileViewModel.Snapshot,
        };
        LauncherSettings.Instance.Data.Profiles.Add(profileData);
        LauncherSettings.Save();
    }
    
    public void ReplaceProfile(string originalProfileName, ProfileViewModel profileViewModel)
    {
        if (LauncherSettings.Instance.Data.Profiles == null)
        {
            SaveProfile(profileViewModel);
            return;
        }

        var editedProfileData = LauncherSettings.Instance.Data.Profiles.Find(p => p.Name == originalProfileName);
        if (editedProfileData == null)
            return;
        
        editedProfileData.Name = profileViewModel.ProfileName;
        editedProfileData.PlayerNickname = profileViewModel.PlayerName;
        editedProfileData.MinecraftVersion = profileViewModel.SelectedVersion?.Id;
        editedProfileData.Alpha = profileViewModel.Alpha;
        editedProfileData.Beta = profileViewModel.Beta;
        editedProfileData.Custom = profileViewModel.Custom;
        editedProfileData.Release = profileViewModel.Release;
        editedProfileData.Snapshot = profileViewModel.Snapshot;
        
        LauncherSettings.Save();
    }
    
    private async void LoadOfflineVersions()
    {
        _offlineVersions = await _minecraftLauncher.GetAvailableVersions(CurrentSettings.Directory);
        if (_offlineVersions.IsEmpty)
            return;
        
        UpdateProgressValues?.Invoke(ProgressLocalizationKeys.Ready, 0, null);
        TryMergeOfflineAndOnlineVersions();
        InvokeVersionsLoaded();
    }
    
    private async void LoadOnlineVersions()
    {
        _onlineVersions = await _minecraftLauncher.GetOnlineAvailableVersions();
        if (_onlineVersions.IsEmpty)
            return;
        
        UpdateProgressValues?.Invoke(ProgressLocalizationKeys.Ready, 0, null);
        TryMergeOfflineAndOnlineVersions();
        InvokeVersionsLoaded();
    }

    private void TryMergeOfflineAndOnlineVersions()
    {
        if (_offlineVersions == null || _onlineVersions == null)
            return;
        
        _allVersions = Versions.Merge(_onlineVersions, _offlineVersions);
        _versionsLoaded?.Invoke(_allVersions);
    }

    private void InvokeVersionsLoaded()
    {
        if (_allVersions != null)
            _versionsLoaded?.Invoke(_allVersions);
        else if (_onlineVersions != null)
            _versionsLoaded?.Invoke(_onlineVersions);
        else if (_offlineVersions != null)
            _versionsLoaded?.Invoke(_offlineVersions);
    }
    
    private bool LoadSettings(out IReadOnlyList<ProfileViewModel> allProfiles, out ProfileViewModel? lastSelectedProfile,
        [NotNullWhen(true)] out SettingsData? settingsData)
    {
        var profiles = new List<ProfileViewModel>();
        
        allProfiles = profiles;
        lastSelectedProfile = null;
        settingsData = null;

        if (!LauncherSettings.Load())
            return false;

        if (LauncherSettings.Instance.Data.Profiles != null)
        {
            _profilesToLoadCount = LauncherSettings.Instance.Data.Profiles.Count;
            for (var i = 0; i < LauncherSettings.Instance.Data.Profiles.Count; i++)
            {
                var profile = LauncherSettings.Instance.Data.Profiles[i];
                var loadedProfile = ProfileViewModel.Load(profile, ProfileLoaded);
                VersionsLoaded += loadedProfile.SetVersions;
                profiles.Add(loadedProfile);
            }
        }
        
        if (!string.IsNullOrEmpty(LauncherSettings.Instance.Data.LastProfileName))
        {
            lastSelectedProfile = profiles.FirstOrDefault(profile =>
                profile.ProfileName == LauncherSettings.Instance.Data.LastProfileName);
        }

        var launcherVisibility = LauncherVisibility.KeepOpen;
        if (Enum.IsDefined(typeof(LauncherVisibility), LauncherSettings.Instance.Data.LauncherVisibility))
        {
            launcherVisibility = (LauncherVisibility)LauncherSettings.Instance.Data.LauncherVisibility;
        }

        var useCustomResolution = LauncherSettings.Instance.Data.UseCustomResolution;
        var screenWidth = LauncherSettings.Instance.Data.ScreenWidth;
        var screenHeight = LauncherSettings.Instance.Data.ScreenHeight;

        if (!string.IsNullOrEmpty(LauncherSettings.Instance.Data.GameDirectory) &&
            DirectoryValidation.IsDirectoryValid(LauncherSettings.Instance.Data.GameDirectory))
        {
            var gameDirectory = LauncherSettings.Instance.Data.GameDirectory;
            
            settingsData = new SettingsData(LauncherSettings.Instance.Data.DefaultPlayerName, gameDirectory,
                launcherVisibility, useCustomResolution, screenHeight, screenWidth);
            
            return true;
        }

        return false;
    }
    
    private void CreateDefaultSettings(out ProfileViewModel profileViewModel, out SettingsData settingsData)
    {
        settingsData = new SettingsData();
        profileViewModel = ProfileViewModel.CreateDefault(SaveProfile);
        profileViewModel.PlayerName = settingsData.DefaultPlayerName;
        VersionsLoaded += profileViewModel.SetVersions;
    }
    
    private void UpdateProgress(LaunchProgress launchProgress, float progress01, string? additionalInfo)
    {
        var key = launchProgress switch
        {
            LaunchProgress.Prepare => ProgressLocalizationKeys.Prepare,
            LaunchProgress.DownloadFiles => ProgressLocalizationKeys.DownloadFiles,
            LaunchProgress.StartGame => ProgressLocalizationKeys.StartGame,
            LaunchProgress.End => ProgressLocalizationKeys.End,
            _ => throw new ArgumentOutOfRangeException(nameof(launchProgress), launchProgress, null)
        };
        
        UpdateProgressValues?.Invoke(key, progress01, additionalInfo);
    }
}