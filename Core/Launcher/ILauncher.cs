using Launcher.PublicData;

namespace Launcher;

internal interface ILauncher
{
    event Action<LaunchProgress, float>? LaunchMinecraftProgress;

    Task<Versions> GetAvailableVersions(CancellationToken cancellationToken);

    Task LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, Action? exitedAction = null);
}