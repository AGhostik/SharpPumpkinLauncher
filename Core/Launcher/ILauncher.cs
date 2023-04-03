using Launcher.PublicData;

namespace Launcher;

internal interface ILauncher
{
    event Action<LaunchProgress, float>? LaunchMinecraftProgress;

    Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken);

    Task<ErrorCode> LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, Action? exitedAction = null);
}