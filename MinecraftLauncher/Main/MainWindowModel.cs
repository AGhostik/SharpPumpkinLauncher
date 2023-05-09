using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Launcher.PublicData;
using MinecraftLauncher.Main.Profile;
using MinecraftLauncher.Main.Progress;
using SimpleLogger;
using SettingsData = MinecraftLauncher.Main.Settings.SettingsData;

namespace MinecraftLauncher.Main;

public sealed class MainWindowModel
{
    private readonly Launcher.MinecraftLauncher _minecraftLauncher;
    private readonly SettingsManager _settingsManager;

    private CancellationTokenSource? _cancellationTokenSource;
    
    public event Action<ProgressLocalizationKeys, float, string?>? Progress;

    public MainWindowModel(Launcher.MinecraftLauncher minecraftLauncher, SettingsManager settingsManager)
    {
        _minecraftLauncher = minecraftLauncher;
        _settingsManager = settingsManager;

        settingsManager.Init();
        Profiles = settingsManager.Profiles;
        LastSelectedProfile = settingsManager.LastSelectedProfile;
    }

    public IReadOnlyList<ProfileViewModel> Profiles { get; }

    public ProfileViewModel? LastSelectedProfile { get; }

    public SettingsData CurrentSettings => _settingsManager.CurrentSettings;

    public async Task StartGame(ProfileViewModel profileViewModel, Action gameExited)
    {
        if (string.IsNullOrEmpty(profileViewModel.PlayerName) || string.IsNullOrEmpty(CurrentSettings.Directory) ||
            profileViewModel.SelectedVersion == null)
        {
            Progress?.Invoke(ProgressLocalizationKeys.InvalidProfile, 0, null);
            return;
        }

        var features = new LaunchFeaturesData(
            CurrentSettings.UseCustomResolution,
            CurrentSettings.ScreenHeight,
            CurrentSettings.ScreenWidth);
        
        var launchData = new LaunchData(
            profileViewModel.PlayerName,
            profileViewModel.SelectedVersion.Id,
            profileViewModel.SelectedForgeVersion?.Id,
            CurrentSettings.Directory,
            features);

        _cancellationTokenSource = new CancellationTokenSource();
        
        _minecraftLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        
        var result = await _minecraftLauncher.LaunchMinecraft(launchData, null, gameExited.Invoke,
                _cancellationTokenSource.Token);
        
        _minecraftLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;

        switch (result)
        {
            case ErrorCode.NoError:
                break;
            case ErrorCode.GameAborted:
                gameExited.Invoke();
                Progress?.Invoke(ProgressLocalizationKeys.Aborted, 0, null);
                break;
            default:
                gameExited.Invoke();
                Progress?.Invoke(ProgressLocalizationKeys.FailToStartGame, 0, null);
                Logger.Log(result);
                break;
        }
        
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    private void OnLaunchMinecraftProgress(LaunchProgress launchProgress, float progress01, string? additionalInfo)
    {
        var key = launchProgress switch
        {
            LaunchProgress.Prepare => ProgressLocalizationKeys.Prepare,
            LaunchProgress.DownloadFiles => ProgressLocalizationKeys.DownloadFiles,
            LaunchProgress.StartGame => ProgressLocalizationKeys.StartGame,
            LaunchProgress.End => ProgressLocalizationKeys.End,
            _ => throw new ArgumentOutOfRangeException(nameof(launchProgress), launchProgress, null)
        };
        
        Progress?.Invoke(key, progress01, additionalInfo);
    }

    public void AbortStartGame()
    {
        _cancellationTokenSource?.Cancel();
    }

    public static void SaveSelectedProfile(string profileName)
    {
        SettingsManager.SaveSelectedProfile(profileName);
    }
}