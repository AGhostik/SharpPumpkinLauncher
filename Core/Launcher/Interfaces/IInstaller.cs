using Launcher.Data;
using Launcher.PublicData;

namespace Launcher.Interfaces;

internal interface IInstaller
{
    event Action<DownloadProgress>? DownloadingProgress;

    Task<MinecraftMissedInfo?> IsVersionInstalled(LaunchData launchData,
        CancellationToken cancellationToken);

    Task<ErrorCode> DownloadAndInstall(LaunchData launchData, MinecraftMissedInfo? minecraftMissedInfo = null,
        CancellationToken cancellationToken = default);
}