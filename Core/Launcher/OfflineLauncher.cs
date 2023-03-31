using Launcher.PublicData;
using Launcher.Tools;

namespace Launcher;

internal sealed class OfflineLauncher : ILauncher
{
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions(CancellationToken cancellationToken)
    {
        //if (FileManager.DirectoryExist())
        
        throw new NotImplementedException();
    }

    public async Task LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, Action? exitedAction = null)
    {
        throw new NotImplementedException();
    }
}