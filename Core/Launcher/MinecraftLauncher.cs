using JsonReader;
using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher
{
    private readonly VersionsLoader _versionsLoader;
    private readonly Installer _installer;
    private readonly GameLauncher _gameLauncher;

    public MinecraftLauncher()
    {
        var jsonManager = new JsonManager();
        _versionsLoader = new VersionsLoader(jsonManager);
        _installer = new Installer(jsonManager);
        _gameLauncher = new GameLauncher(jsonManager);
    }

    public event Action<LaunchProgress, float, string?>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken = default)
    {
        var offlineVersions = await _versionsLoader.GetInstalledVersions(directory, cancellationToken);

        if (!await DownloadManager.CheckConnection(cancellationToken))
            return offlineVersions;
        
        var onlineVersions = await _versionsLoader.GetOnlineAvailableVersions(cancellationToken);
        
        onlineVersions.Merge(offlineVersions);
        return onlineVersions;
    }

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, 
        Action? startedAction = null, Action? exitedAction = null)
    {
        OnLaunchMinecraftProgress(LaunchProgress.Prepare);
        
        if (!launchData.Version.IsInstalled)
        {
            _installer.DownloadingProgress += InstallerOnDownloadingProgress;
            var installResult = await _installer.DownloadAndInstall(launchData, cancellationToken);
            _installer.DownloadingProgress -= InstallerOnDownloadingProgress;
            
            if (installResult is not ErrorCode.NoError)
                return installResult;
        }

        _gameLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        var firstLaunchResult =
            await _gameLauncher.LaunchMinecraft(launchData, cancellationToken, startedAction, exitedAction);
        _gameLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;

        if (firstLaunchResult is ErrorCode.NoError)
            return ErrorCode.NoError;

        if (firstLaunchResult is ErrorCode.GameAborted or ErrorCode.StartProcess)
            return firstLaunchResult;

        _installer.DownloadingProgress += InstallerOnDownloadingProgress;
        var installAfterLaunchFailResult = await _installer.DownloadAndInstall(launchData, cancellationToken);
        _installer.DownloadingProgress -= InstallerOnDownloadingProgress;
        
        if (installAfterLaunchFailResult is not ErrorCode.NoError)
            return installAfterLaunchFailResult;

        _gameLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        var secondLaunchResult =
            await _gameLauncher.LaunchMinecraft(launchData, cancellationToken, startedAction, exitedAction);
        _gameLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;

        return secondLaunchResult;
    }

    private void InstallerOnDownloadingProgress(DownloadProgress downloadProgress)
    {
        var progress = (float)downloadProgress.BytesReceived / downloadProgress.TotalSizeInBytes;
        var additionalInfo = $" ({downloadProgress.FilesDownloaded} / {downloadProgress.TotalFilesCount})";
        LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, progress, additionalInfo);
    }

    private void OnLaunchMinecraftProgress(LaunchProgress progress)
    {
        LaunchMinecraftProgress?.Invoke(progress, 0f, null);
    }
}