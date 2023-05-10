using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Launcher.PublicData;
using SharpPumpkinLauncher.Main.Profile;
using SharpPumpkinLauncher.Main.Progress;
using SharpPumpkinLauncher.Properties;
using SimpleLogger;
using SettingsData = SharpPumpkinLauncher.Main.Settings.SettingsData;

namespace SharpPumpkinLauncher.Main;

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
            InvokeSimpleProgress(ProgressLocalizationKeys.InvalidProfile);
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
            case ErrorCode.Aborted:
                gameExited.Invoke();
                InvokeSimpleProgress(ProgressLocalizationKeys.Aborted);
                break;
            default:
                gameExited.Invoke();
                InvokeSimpleProgress(ProgressLocalizationKeys.FailToStartGame);
                Logger.Log(result);
                break;
        }
        
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    private void InvokeSimpleProgress(ProgressLocalizationKeys key)
    {
        Progress?.Invoke(key, -1, null);
    }
    
    private void OnLaunchMinecraftProgress(LaunchProgress launchProgress, DownloadProgress? downloadProgress,
        ForgeInstallProfileProgress? forgeInstallProfileProgress)
    {
        var key = launchProgress switch
        {
            LaunchProgress.Prepare => ProgressLocalizationKeys.Prepare,
            LaunchProgress.DownloadFiles => ProgressLocalizationKeys.DownloadFiles,
            LaunchProgress.InstallForge => ProgressLocalizationKeys.InstallForge,
            LaunchProgress.StartGame => ProgressLocalizationKeys.StartGame,
            LaunchProgress.End => ProgressLocalizationKeys.End,
            _ => throw new ArgumentOutOfRangeException(nameof(launchProgress), launchProgress, null)
        };

        float progress01;
        string? additionalInfo;
        if (downloadProgress.HasValue)
        {
            var value = downloadProgress.Value;

            var speed = GetSpeedString(value.BytesPerSecond);
            progress01 = (float)value.BytesReceived / value.TotalSizeInBytes;
            additionalInfo = $" ({Localization.DownloadFilesLeft}: {value.TotalFilesCount - value.FilesDownloaded}) ({speed})";
        }
        else if (forgeInstallProfileProgress.HasValue)
        {
            var value = forgeInstallProfileProgress.Value;
            progress01 = (float)value.CurrentProcessor / value.TotalProcessor;
            additionalInfo = $" ({Localization.StepsLeft}: {value.TotalProcessor - value.CurrentProcessor})";
        }
        else
        { 
            progress01 = -1;
            additionalInfo = null;
        }
        
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

    private static string GetSpeedString(float speed)
    {
        if (speed > 1024 * 1024)
        {
            return $"{speed / (1024 * 1024):F2} {Localization.MbPerSecond}";
        }

        if (speed > 1024 )
        {
            return $"{speed / 1024:F2} {Localization.KbPerSecond}";
        }

        return $"{speed:F2} {Localization.BytesPerSecond}";
    }
}