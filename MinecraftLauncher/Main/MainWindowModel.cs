using System;
using System.Collections.Generic;
using System.Linq;
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

    private Versions? _availableVersions;
    private Action<Versions>? _versionsLoaded;

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

    private int _profilesToLoadCount;
    private int _currentLoadedProfilesCount;
    public event Action? AllProfilesLoaded;

    public MainWindowModel()
    {
        _minecraftLauncher = new MinecraftLauncher();
        _minecraftLauncher.LaunchMinecraftProgress +=
            (status, progress01) => StartGameProgress?.Invoke(status, progress01);
    }

    public async Task InitAsync()
    {
        var availableVersions = await _minecraftLauncher.GetAvailableVersions();
        _availableVersions = availableVersions;
        _versionsLoaded?.Invoke(availableVersions);
    }

    public async Task StartGame(ProfileViewModel profileViewModel, string directory, Action gameExited)
    {
        if (string.IsNullOrEmpty(profileViewModel.PlayerName) ||
            string.IsNullOrEmpty(profileViewModel.SelectedVersion?.Id) ||
            string.IsNullOrEmpty(directory))
            return;
        
        var launchData = new LaunchData(
            profileViewModel.PlayerName,
            profileViewModel.SelectedVersion.Id,
            directory);
        
        await _minecraftLauncher.LaunchMinecraft(launchData, gameExited);
    }

    public void SaveSelectedProfile(string profileName)
    {
        LauncherSettings.Instance.Data.LastProfileName = profileName;
        LauncherSettings.Save();
    }

    public bool LoadSettings(out IReadOnlyList<ProfileViewModel> allProfiles, out ProfileViewModel? lastSelectedProfile,
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

    private void ProfileLoaded(ProfileViewModel profileViewModel)
    {
        _currentLoadedProfilesCount++;
        if (_currentLoadedProfilesCount == _profilesToLoadCount)
            AllProfilesLoaded?.Invoke();
    }

    public void SaveSettings(SettingsData settingsData)
    {
        LauncherSettings.Instance.Data.LauncherVisibility = (int)settingsData.LauncherVisibility;
        LauncherSettings.Instance.Data.GameDirectory = settingsData.Directory;
        LauncherSettings.Instance.Data.DefaultPlayerName = settingsData.DefaultPlayerName;
        LauncherSettings.Save();
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
}