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
    private readonly VersionsLoader _versionsLoader;
    private readonly SettingsManager _settingsManager;

    private CancellationTokenSource? _cancellationTokenSource;

    public event Action<ProgressLocalizationKeys, float, string?>? UpdateProgressValues;

    public MainWindowModel(Launcher.MinecraftLauncher minecraftLauncher, VersionsLoader versionsLoader,
        SettingsManager settingsManager)
    {
        _minecraftLauncher = minecraftLauncher;
        _versionsLoader = versionsLoader;
        _settingsManager = settingsManager;

        settingsManager.Init();
        Profiles = settingsManager.Profiles;
        LastSelectedProfile = settingsManager.LastSelectedProfile;

        _minecraftLauncher.LaunchMinecraftProgress += UpdateProgress;
        _versionsLoader.VersionsLoaded += OnVersionsLoaded;
    }

    public IReadOnlyList<ProfileViewModel> Profiles { get; }

    public ProfileViewModel? LastSelectedProfile { get; }

    public SettingsData CurrentSettings => _settingsManager.CurrentSettings;

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
            profileViewModel.SelectedVersion.Id,
            profileViewModel.SelectedForgeVersion?.Id,
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
    
    private void OnVersionsLoaded(Versions _)
    {
        UpdateProgressValues?.Invoke(ProgressLocalizationKeys.Ready, 0, null);
        _versionsLoader.VersionsLoaded -= OnVersionsLoaded;
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

    public static void SaveSelectedProfile(string profileName)
    {
        SettingsManager.SaveSelectedProfile(profileName);
    }
}