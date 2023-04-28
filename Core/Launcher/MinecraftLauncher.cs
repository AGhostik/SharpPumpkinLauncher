using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher : ILauncher
{
    private readonly OfflineLauncher _offlineLauncher = new();
    private readonly OnlineLauncher _onlineLauncher = new();
    
    private ILauncher? _launcher;
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken = default)
    {
        var offlineVersions = await _offlineLauncher.GetAvailableVersions(directory, cancellationToken);

        if (!await DownloadManager.CheckConnection(cancellationToken))
            return offlineVersions;
        
        var onlineVersions = await _onlineLauncher.GetAvailableVersions(directory, cancellationToken);
        onlineVersions.Merge(offlineVersions);
        return onlineVersions;
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