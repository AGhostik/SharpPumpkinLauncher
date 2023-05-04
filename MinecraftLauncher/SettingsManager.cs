using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MinecraftLauncher.Main.Profile;
using MinecraftLauncher.Main.Settings;
using MinecraftLauncher.Main.Validation;
using UserSettings;
using SettingsData = MinecraftLauncher.Main.Settings.SettingsData;

namespace MinecraftLauncher;

public class SettingsManager
{
    public event Action<string>? DirectoryChanged;

    public IReadOnlyList<ProfileViewModel> Profiles { get; private set; } = Array.Empty<ProfileViewModel>();

    public ProfileViewModel? LastSelectedProfile { get; private set; }
    
    public SettingsData CurrentSettings { get; private set; } = new();
    
    public void Init()
    {
        if (LoadSettingsInternal(out var allProfiles, out var lastSelectedProfile, out var settings))
        {
            CurrentSettings = settings;
            Profiles = allProfiles;
            LastSelectedProfile = lastSelectedProfile;
        }
        else
        {
            Profiles = Array.Empty<ProfileViewModel>();
            LastSelectedProfile = null;
            
            SaveSettingsData(new SettingsData());
        }
    }
    
    public void SaveSettingsData(SettingsData settingsData)
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
            DirectoryChanged?.Invoke(settingsData.Directory);
    }

    public static void SaveSelectedProfile(string profileName)
    {
        LauncherSettings.Instance.Data.LastProfileName = profileName;
        LauncherSettings.Save();
    }
    
    public static void SaveProfile(ProfileViewModel profileViewModel)
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
            ForgeVersion = profileViewModel.SelectedForgeVersion?.Id,
            Alpha = profileViewModel.Alpha,
            Beta = profileViewModel.Beta,
            Custom = profileViewModel.Custom,
            Release = profileViewModel.Release,
            Snapshot = profileViewModel.Snapshot,
            Forge = profileViewModel.Forge
        };

        if (profileViewModel.SelectedVersion != null)
            profileData.MinecraftVersionTags = new List<string>(profileViewModel.SelectedVersion.Tags);
        
        if (profileViewModel.SelectedForgeVersion != null)
            profileData.ForgeVersionTags = new List<string>(profileViewModel.SelectedForgeVersion.Tags);
        
        LauncherSettings.Instance.Data.Profiles.Add(profileData);
        LauncherSettings.Save();
    }
    
    public static void ReplaceProfile(string originalProfileName, ProfileViewModel profileViewModel)
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
        editedProfileData.ForgeVersion = profileViewModel.SelectedForgeVersion?.Id;
        editedProfileData.Alpha = profileViewModel.Alpha;
        editedProfileData.Beta = profileViewModel.Beta;
        editedProfileData.Custom = profileViewModel.Custom;
        editedProfileData.Release = profileViewModel.Release;
        editedProfileData.Snapshot = profileViewModel.Snapshot;
        
        if (profileViewModel.SelectedVersion != null)
            editedProfileData.MinecraftVersionTags = new List<string>(profileViewModel.SelectedVersion.Tags);
        
        if (profileViewModel.SelectedForgeVersion != null)
            editedProfileData.ForgeVersionTags = new List<string>(profileViewModel.SelectedForgeVersion.Tags);
        
        LauncherSettings.Save();
    }
    
    public static void DeleteProfile(ProfileViewModel profileViewModel)
    {
        if (LauncherSettings.Instance.Data.Profiles == null)
            return;

        var profileToDelete = LauncherSettings.Instance.Data.Profiles.Find(p => p.Name == profileViewModel.ProfileName);
        if (profileToDelete == null)
            return;
        
        LauncherSettings.Instance.Data.Profiles.Remove(profileToDelete);
        LauncherSettings.Save();
    }

    private static bool LoadSettingsInternal(out IReadOnlyList<ProfileViewModel> allProfiles,
        out ProfileViewModel? lastSelectedProfile, [NotNullWhen(true)] out SettingsData? settingsData)
    {
        var profiles = new List<ProfileViewModel>();
        
        allProfiles = profiles;
        lastSelectedProfile = null;
        settingsData = null;

        if (!LauncherSettings.Load())
            return false;

        if (LauncherSettings.Instance.Data.Profiles != null)
        {
            for (var i = 0; i < LauncherSettings.Instance.Data.Profiles.Count; i++)
            {
                var profile = LauncherSettings.Instance.Data.Profiles[i];
                var loadedProfile = ProfileViewModel.Load(profile);
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
}