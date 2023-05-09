using Launcher.PublicData;

namespace Launcher.Interfaces;

internal interface IGameLauncher
{
    event Action<LaunchProgress>? LaunchMinecraftProgress;

    Task<ErrorCode> LaunchMinecraft(LaunchData launchData, Action? startedAction = null,
        Action? exitedAction = null, CancellationToken cancellationToken = default);
}