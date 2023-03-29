using Launcher.PublicData;

namespace Launcher;

internal sealed class OfflineLauncher : ILauncher
{
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions()
    {
        throw new NotImplementedException();
    }

    public async Task LaunchMinecraft(LaunchData launchData, Action? exitedAction = null)
    {
        throw new NotImplementedException();
    }
}