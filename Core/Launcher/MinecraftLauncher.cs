using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher : ILauncher
{
    private ILauncher? _launcher;
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions(CancellationToken cancellationToken = default)
    {
        var launcher = await GetLauncher();
        return await launcher.GetAvailableVersions(cancellationToken);
    }

    public async Task LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken, Action? exitedAction = null)
    {
        var launcher = await GetLauncher();
        await launcher.LaunchMinecraft(launchData, cancellationToken, exitedAction);
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