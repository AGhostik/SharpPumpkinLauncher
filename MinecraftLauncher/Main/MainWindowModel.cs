using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Launcher.PublicData;
using MinecraftLauncher.Main.Profile;
using UserSettings;

namespace MinecraftLauncher.Main;
using Launcher;

public sealed class MainWindowModel
{
    private readonly MinecraftLauncher _minecraftLauncher;

    private Action? _versionsLoaded;
    public event Action? VersionsLoaded
    {
        add
        {
            if (AvailableVersions != null)
                value?.Invoke();
            else
                _versionsLoaded += value;
        }
        remove => _versionsLoaded -= value;
    }

    public event Action<LaunchProgress, float>? StartGameProgress;

    public MainWindowModel()
    {
        _minecraftLauncher = new MinecraftLauncher();
        _minecraftLauncher.LaunchMinecraftProgress +=
            (status, progress01) => StartGameProgress?.Invoke(status, progress01);
    }

    public async Task InitAsync()
    {
        AvailableVersions = await _minecraftLauncher.GetAvailableVersions();
        _versionsLoaded?.Invoke();
    }

    public Versions? AvailableVersions { get; private set; }

    public async Task StartGame(ProfileViewModel profileViewModel)
    {
        if (string.IsNullOrEmpty(profileViewModel.PlayerName) ||
            string.IsNullOrEmpty(profileViewModel.SelectedVersion?.Id) ||
            string.IsNullOrEmpty(profileViewModel.Directory))
            return;
        
        var launchData = new LaunchData(
            profileViewModel.PlayerName,
            profileViewModel.SelectedVersion.Id,
            profileViewModel.Directory);
        
        await _minecraftLauncher.LaunchMinecraft(launchData);
    }

    public void SaveSelectedProfile(string profileName)
    {
        LauncherSettings.Instance.Data.LastProfileName = profileName;
        LauncherSettings.Save();
    }

    public string? GetLastSelectedProfile()
    {
        if (!LauncherSettings.Load())
            return null;

        return LauncherSettings.Instance.Data.LastProfileName;
    }

    public IReadOnlyList<ProfileViewModel> GetProfiles()
    {
        if (!LauncherSettings.Load() || AvailableVersions == null)
            return Array.Empty<ProfileViewModel>();

        var result = new List<ProfileViewModel>();
        if (LauncherSettings.Instance.Data.Profiles != null)
        {
            for (var i = 0; i < LauncherSettings.Instance.Data.Profiles.Count; i++)
            {
                var profile = LauncherSettings.Instance.Data.Profiles[i];
                result.Add(ProfileViewModel.Load(profile, AvailableVersions));
            }
        }

        return result;
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
            GameDirectory = profileViewModel.Directory,
            MinecraftVersion = profileViewModel.SelectedVersion?.Id
        };
        LauncherSettings.Instance.Data.Profiles.Add(profileData);
        LauncherSettings.Save();
    }
}