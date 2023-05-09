using JsonReader;
using Launcher.Forge;
using Launcher.Interfaces;
using Launcher.PublicData;
using Launcher.Tools;
using Launcher.Vanilla;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher
{
    private readonly VanillaInstaller _vanillaInstaller;
    private readonly VanillaGameLauncher _vanillaGameLauncher;
    private readonly VanillaVersionsLoader _vanillaVersionsLoader;
    
    private readonly ForgeInstaller _forgeInstaller;
    private readonly ForgeGameLauncher _forgeGameLauncher;
    private readonly ForgeVersionsLoader _forgeVersionsLoader;

    public MinecraftLauncher()
    {
        var jsonManager = new JsonManager();

        var installerData = new InstallerData(jsonManager);
        var baseInstaller = new BaseInstaller();
        
        _vanillaVersionsLoader = new VanillaVersionsLoader(jsonManager);
        _vanillaInstaller = new VanillaInstaller(baseInstaller, _vanillaVersionsLoader, installerData);
        _vanillaGameLauncher = new VanillaGameLauncher(installerData);

        _forgeVersionsLoader = new ForgeVersionsLoader(jsonManager);
        _forgeInstaller = new ForgeInstaller(baseInstaller, _vanillaVersionsLoader, _forgeVersionsLoader, installerData);
        _forgeGameLauncher = new ForgeGameLauncher(installerData);
    }

    public event Action<LaunchProgress, float, string?>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken = default)
    {
        return await _vanillaVersionsLoader.ReadVersionsFromDisk(directory, cancellationToken);
    }
    
    public async Task<Versions> GetOnlineAvailableVersions(CancellationToken cancellationToken = default)
    {
        return await _vanillaVersionsLoader.GetOnlineAvailableVersions(cancellationToken);
    }
    
    public async Task<ForgeVersions> GetOnlineForgeVersions(string versionId,
        CancellationToken cancellationToken = default)
    {
        return await _forgeVersionsLoader.GetOnlineForgeVersions(versionId, cancellationToken);
    }

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, Action? startedAction = null, 
        Action? exitedAction = null, CancellationToken cancellationToken = default)
    {
        IInstaller installer;
        IGameLauncher gameLauncher;
        if (string.IsNullOrEmpty(launchData.ForgeVersionId))
        {
            installer = _vanillaInstaller;
            gameLauncher = _vanillaGameLauncher;
        }
        else
        {
            installer = _forgeInstaller;
            gameLauncher = _forgeGameLauncher;
        }
        
        return await LaunchMinecraftInternal(launchData, installer, gameLauncher, startedAction,
            exitedAction, cancellationToken);
    }
    
    private async Task<ErrorCode> LaunchMinecraftInternal(LaunchData launchData, IInstaller installer, 
        IGameLauncher gameLauncher, Action? startedAction = null, Action? exitedAction = null, 
        CancellationToken cancellationToken = default)
    {
        OnLaunchMinecraftProgress(LaunchProgress.Prepare);

        var minecraftMissedInfo = await installer.IsVersionInstalled(launchData, cancellationToken);
        if (minecraftMissedInfo == null || !minecraftMissedInfo.IsDownloadingNotNeeded)
        {
            installer.DownloadingProgress += OnDownloadingProgress;
            var installResult = await installer.DownloadAndInstall(launchData, minecraftMissedInfo, cancellationToken);
            installer.DownloadingProgress -= OnDownloadingProgress;

            if (installResult is not ErrorCode.NoError)
            {
                if (cancellationToken.IsCancellationRequested)
                    return ErrorCode.GameAborted;
                
                return installResult;
            }
        }

        gameLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        var launchResult =
            await gameLauncher.LaunchMinecraft(launchData, startedAction, exitedAction, cancellationToken);
        gameLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;

        if (cancellationToken.IsCancellationRequested)
            return ErrorCode.GameAborted;
        
        return launchResult;
    }

    private void OnDownloadingProgress(DownloadProgress downloadProgress)
    {
        var progress = (float)downloadProgress.BytesReceived / downloadProgress.TotalSizeInBytes;
        var additionalInfo = $" ({downloadProgress.TotalFilesCount - downloadProgress.FilesDownloaded})";
        LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, progress, additionalInfo);
    }

    private void OnLaunchMinecraftProgress(LaunchProgress progress)
    {
        LaunchMinecraftProgress?.Invoke(progress, 0f, null);
    }
}