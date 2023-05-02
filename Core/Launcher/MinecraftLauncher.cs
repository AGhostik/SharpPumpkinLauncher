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
        return await _versionsLoader.ReadVersionsFromDisk(directory, cancellationToken);
    }
    
    public async Task<Versions> GetOnlineAvailableVersions(CancellationToken cancellationToken = default)
    {
        return await _versionsLoader.GetOnlineAvailableVersions(cancellationToken);
    }

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, 
        Action? startedAction = null, Action? exitedAction = null)
    {
        OnLaunchMinecraftProgress(LaunchProgress.Prepare);

        var minecraftMissedInfo = await _gameLauncher.IsVersionInstalled(launchData, cancellationToken);
        if (minecraftMissedInfo == null || !minecraftMissedInfo.IsEmpty)
        {
            //case when online versions not loaded yet
            if (string.IsNullOrEmpty(launchData.Version.Url))
                return ErrorCode.NeedVersionUrl;
            
            _installer.DownloadingProgress += InstallerOnDownloadingProgress;
            var installResult = await _installer.DownloadAndInstall(launchData, cancellationToken);
            _installer.DownloadingProgress -= InstallerOnDownloadingProgress;

            if (installResult is not ErrorCode.NoError)
                return installResult;
        }

        _gameLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        var launchResult =
            await _gameLauncher.LaunchMinecraft(launchData, cancellationToken, startedAction, exitedAction);
        _gameLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;

        return launchResult;
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