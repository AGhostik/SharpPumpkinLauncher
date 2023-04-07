using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher : ILauncher
{
    private ILauncher? _launcher;
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken = default)
    {
        var launcher = await GetLauncher();
        return await launcher.GetAvailableVersions(directory, cancellationToken);
    }

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, 
        Action? startedAction = null, Action? exitedAction = null)
    {
        var launcher = await GetLauncher();
        return await launcher.LaunchMinecraft(launchData, cancellationToken, startedAction, exitedAction);
    }

    private async Task<ILauncher> GetLauncher()
    {
        if (_launcher != null)
            return _launcher;
        
        if (await DownloadManager.CheckConnection())
            _launcher = new OnlineLauncher();
        else
            _launcher = new OfflineLauncher();

        _launcher.LaunchMinecraftProgress += (progress, f) => LaunchMinecraftProgress?.Invoke(progress, f);
        return _launcher;
    }
}