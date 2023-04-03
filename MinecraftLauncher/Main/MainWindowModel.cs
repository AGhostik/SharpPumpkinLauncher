using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Launcher.PublicData;
using MinecraftLauncher.Main.Profile;
using MinecraftLauncher.Main.Settings;
using MinecraftLauncher.Main.Validation;
using UserSettings;
using SettingsData = MinecraftLauncher.Main.Settings.SettingsData;

namespace MinecraftLauncher.Main;
using Launcher;

public sealed class MainWindowModel
{
    private readonly MinecraftLauncher _minecraftLauncher;

    private CancellationTokenSource? _cancellationTokenSource;

    private Versions? _availableVersions;
    private Action<Versions>? _versionsLoaded;
    private int _profilesToLoadCount;
    private int _currentLoadedProfilesCount;

    public event Action<Versions>? VersionsLoaded
    {
        add
        {
            if (_availableVersions != null)
                value?.Invoke(_availableVersions);
            
            _versionsLoaded += value;
        }
        remove => _versionsLoaded -= value;
    }

    public event Action<LaunchProgress, float>? StartGameProgress;
    public event Action? AllProfilesLoaded;

    private event Action SettingsDirectorySet;

    public MainWindowModel()
    {
        _minecraftLauncher = new MinecraftLauncher();
        _minecraftLauncher.LaunchMinecraftProgress +=
            (status, progress01) => StartGameProgress?.Invoke(status, progress01);

        SettingsDirectorySet += LoadAvailableVersions;

        if (LoadSettings(out var allProfiles, out var lastSelectedProfile, out var settingsData) && settingsData != null)
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
        }
        
        SettingsDirectorySet.Invoke();
    }

    public IReadOnlyList<ProfileViewModel> Profiles { get; }

    public ProfileViewModel? LastSelectedProfile { get; }

    public SettingsData CurrentSettings { get; private set; }

    public async Task StartGame(ProfileViewModel profileViewModel, Action gameExited)
    {
        if (string.IsNullOrEmpty(profileViewModel.PlayerName) ||
            string.IsNullOrEmpty(CurrentSettings.Directory) ||
            profileViewModel.SelectedVersion == null)
            return;
        
        var launchData = new LaunchData(
            profileViewModel.PlayerName,
            CurrentSettings.Directory,
            profileViewModel.SelectedVersion);

        _cancellationTokenSource = new CancellationTokenSource();
        
        var result = await _minecraftLauncher.LaunchMinecraft(launchData, _cancellationTokenSource.Token, gameExited.Invoke);

        if (result != ErrorCode.NoError)
        {
            gameExited.Invoke();
            Debug.WriteLine(result);
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
        var needInvokeEvent = CurrentSettings.Directory != settingsData.Directory;
        CurrentSettings = settingsData;

        LauncherSettings.Instance.Data.LauncherVisibility = (int)settingsData.LauncherVisibility;
        LauncherSettings.Instance.Data.GameDirectory = settingsData.Directory;
        LauncherSettings.Instance.Data.DefaultPlayerName = settingsData.DefaultPlayerName;
        LauncherSettings.Save();
        
        if (needInvokeEvent)
            SettingsDirectorySet.Invoke();
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
    
    private async void LoadAvailableVersions()
    {
        var availableVersions = await _minecraftLauncher.GetAvailableVersions(CurrentSettings.Directory);
        _availableVersions = availableVersions;
        _versionsLoaded?.Invoke(availableVersions);
    }
    
    private bool LoadSettings(out IReadOnlyList<ProfileViewModel> allProfiles, out ProfileViewModel? lastSelectedProfile,
        out SettingsData? settingsData)
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

        var gameDirectory  = string.Empty;
        if (!string.IsNullOrEmpty(LauncherSettings.Instance.Data.GameDirectory) &&
            DirectoryValidation.IsDirectoryValid(LauncherSettings.Instance.Data.GameDirectory))
        {
            gameDirectory = LauncherSettings.Instance.Data.GameDirectory;
        }

        settingsData = new SettingsData(LauncherSettings.Instance.Data.DefaultPlayerName, gameDirectory, launcherVisibility);
        
        return true;
    }
    
    private void CreateDefaultSettings(out ProfileViewModel profileViewModel, out SettingsData settingsData)
    {
        settingsData = new SettingsData();
        profileViewModel = ProfileViewModel.CreateDefault(SaveProfile);
        profileViewModel.PlayerName = settingsData.DefaultPlayerName;
        VersionsLoaded += profileViewModel.SetVersions;
    }
}