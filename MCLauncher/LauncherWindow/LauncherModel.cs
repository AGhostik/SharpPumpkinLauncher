using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Launcher;
using Launcher.PublicData;
using MCLauncher.Messages;
using MCLauncher.SettingsWindow;
using Microsoft.Win32;
using UserSettings;

namespace MCLauncher.LauncherWindow;

public class LauncherModel
{
    private readonly MinecraftLauncher _minecraftLauncher;
    private readonly MinecraftData _minecraftData;
    private readonly List<ProfileViewModel> _profiles = new();

    public event Action<ProfileViewModel>? ProfileAdded;
    public event Action? MinecraftDataLoaded
    {
        add
        {
            if (_minecraftData.IsVersionsLoaded)
                value?.Invoke();
            else
                _minecraftData.VersionsLoaded += value;
        }
        
        remove => _minecraftData.VersionsLoaded -= value;
    }
    public event MinecraftLauncher.ProgressDelegate? LaunchProgress; 

    public LauncherModel(MinecraftLauncher minecraftLauncher, MinecraftData minecraftData)
    {
        _minecraftLauncher = minecraftLauncher;
        _minecraftData = minecraftData;

        LauncherSettings.Load();
        var profiles = LauncherSettings.Instance.Data.Profiles;
        if (profiles != null)
        {
            for (var i = 0; i < profiles.Count; i++)
                _profiles.Add(new ProfileViewModel(profiles[i]));
        }

        minecraftLauncher.LaunchMinecraftProgress += MinecraftLauncherOnLaunchMinecraftProgress;
        
        WeakReferenceMessenger.Default.Register<ProfileSaved>(this, ProfileSavedMessageHandler);
    }

    private void MinecraftLauncherOnLaunchMinecraftProgress(string status, float progress01)
    {
        LaunchProgress?.Invoke(status, progress01);
    }

    private void ProfileSavedMessageHandler(object recipient, ProfileSaved message)
    {
        var newProfile = new ProfileViewModel(message.ProfileData);
        _profiles.Add(newProfile);

        LauncherSettings.Instance.Data.Profiles?.Add(message.ProfileData);
        LauncherSettings.Save();
        
        ProfileAdded?.Invoke(newProfile);
    }

    public IReadOnlyList<ProfileViewModel> GetProfiles()
    {
        return _profiles;
    }

    public ProfileViewModel? GetLastProfile()
    {
        if (LauncherSettings.Instance.Data.Profiles == null)
            return null;
        
        var lastProfileName = LauncherSettings.Instance.Data.LastProfileName;

        if (string.IsNullOrEmpty(lastProfileName))
            return null;

        var lastProfile = _profiles.Find(profile => profile.Name == lastProfileName);

        return lastProfile;
    }
    
    public void SaveLastProfile(ProfileViewModel profile)
    {
        LauncherSettings.Instance.Data.LastProfileName = profile.Name;
    }
    
    public void CreateProfile()
    {
        WeakReferenceMessenger.Default.Send(new ShowSettingsMessage(null));
    }

    public void EditProfile(ProfileViewModel? profile)
    {
        if (profile == null)
            return;

        var profileToEdit = _profiles.Find(profileData => profileData.Name == profile.Name);
        if (profileToEdit == null)
            return;

        WeakReferenceMessenger.Default.Send(new ShowSettingsMessage(profileToEdit));
    }
    
    public void DeleteProfile(ProfileViewModel? profile)
    {
        if (profile == null)
            return;
        
        if (LauncherSettings.Instance.Data.Profiles == null)
            return;
        
        var profileToDelete = LauncherSettings.Instance.Data.Profiles.Find(profileData => profileData.Name == profile.Name);
        if (profileToDelete != null)
            LauncherSettings.Instance.Data.Profiles.Remove(profileToDelete);

        _profiles.Remove(profile);
    }

    public async Task StartGame(ProfileViewModel? profile, Action? afterExit = null)
    {
        if (profile == null)
        {
            afterExit?.Invoke();
            return;
        }
        
        if (string.IsNullOrEmpty(profile.JavaFile) ||
            string.IsNullOrEmpty(profile.PlayerNickname) ||
            string.IsNullOrEmpty(profile.MinecraftVersion) ||
            string.IsNullOrEmpty(profile.GameDirectory))
        {
            afterExit?.Invoke();
            return;
        }

        var launchData = new LaunchData(profile.PlayerNickname, profile.MinecraftVersion, profile.GameDirectory,
            profile.JavaFile);

        Action? exitedAction = null;

        var launcherVisibility = (LauncherVisibility)LauncherSettings.Instance.Data.LauncherVisibility;
        switch (launcherVisibility)
        {
            case LauncherVisibility.Close:
                Application.Current?.MainWindow?.Close();
                break;

            case LauncherVisibility.Hide:
                Application.Current?.MainWindow?.Hide();
                exitedAction = () =>
                {
                    Application.Current?.MainWindow?.Show();
                    afterExit?.Invoke();
                };
                break;

            default:
            case LauncherVisibility.KeepOpen:
                exitedAction = afterExit;
                break;
        }

        await _minecraftLauncher.LaunchMinecraft(launchData, exitedAction);
    }
}