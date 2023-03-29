using Launcher.PublicData;

namespace Launcher;

internal interface ILauncher
{
    event Action<LaunchProgress, float>? LaunchMinecraftProgress;

    Task<Versions> GetAvailableVersions();

    Task LaunchMinecraft(LaunchData launchData, Action? exitedAction = null);
}