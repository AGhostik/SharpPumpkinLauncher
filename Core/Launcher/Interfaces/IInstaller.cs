using Launcher.Data;
using Launcher.PublicData;

namespace Launcher.Interfaces;

internal interface IInstaller
{
    event Action<DownloadProgress>? DownloadingProgress;

    Task<MinecraftMissedInfo?> IsVersionInstalled(LaunchData launchData,
        CancellationToken cancellationToken);

    Task<ErrorCode> DownloadAndInstall(string versionId, string gameDirectory,
        MinecraftMissedInfo? minecraftMissedInfo = null, CancellationToken cancellationToken = default);
}