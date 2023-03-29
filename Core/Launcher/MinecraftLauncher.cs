using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher : ILauncher
{
    private ILauncher? _launcher;
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public async Task<Versions> GetAvailableVersions()
    {
        var launcher = await GetLauncher();
        return await launcher.GetAvailableVersions();
    }

    public async Task LaunchMinecraft(LaunchData launchData, Action? exitedAction = null)
    {
        var launcher = await GetLauncher();
        await launcher.LaunchMinecraft(launchData, exitedAction);
    }

    private async Task<ILauncher> GetLauncher()
    {
        if (_launcher != null)
            return _launcher;
        
        if (await FileManager.CheckConnection())
            _launcher = new OnlineLauncher();
        else
            _launcher = new OfflineLauncher();

        _launcher.LaunchMinecraftProgress += (progress, f) => LaunchMinecraftProgress?.Invoke(progress, f);
        return _launcher;
    }
}