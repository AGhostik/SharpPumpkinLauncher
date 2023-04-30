using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public sealed class MinecraftLauncher : ILauncher
{
    private readonly OfflineLauncher _offlineLauncher = new();
    private readonly OnlineLauncher _onlineLauncher = new();
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
        // if (launchData.Version.IsInstalled)
        // {
        //     _offlineLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        //     var offlineLaunchResult =
        //         await _offlineLauncher.LaunchMinecraft(launchData, cancellationToken, startedAction, exitedAction);
        //     
        //     _offlineLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;
        //     
        //     if (offlineLaunchResult is ErrorCode.NoError or ErrorCode.GameAborted or ErrorCode.StartProcess)
        //         return offlineLaunchResult;
        // }

        if (!await DownloadManager.CheckConnection(cancellationToken))
            return ErrorCode.Connection;
        
        _onlineLauncher.LaunchMinecraftProgress += OnLaunchMinecraftProgress;
        var result = await _onlineLauncher.LaunchMinecraft(launchData, cancellationToken, startedAction, exitedAction);
        _onlineLauncher.LaunchMinecraftProgress -= OnLaunchMinecraftProgress;
        
        return result;
    }

    private void OnLaunchMinecraftProgress(LaunchProgress progress, float f)
    {
        LaunchMinecraftProgress?.Invoke(progress, f);
    }
}