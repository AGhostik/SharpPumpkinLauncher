using Launcher.Interfaces;
using Launcher.PublicData;
using Launcher.Tools;
using SimpleLogger;

namespace Launcher.Vanilla;

internal sealed class VanillaGameLauncher : IGameLauncher
{
    private readonly IInstallerData _installerData;

    public VanillaGameLauncher(IInstallerData installerData)
    {
        _installerData = installerData;
    }
    
    public event Action<LaunchProgress>? LaunchMinecraftProgress;

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, Action? startedAction = null, 
        Action? exitedAction = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var versionId = launchData.VersionId;
            var gameDirectory = launchData.GameDirectory;
            var playerName = launchData.PlayerName;
            var featuresData = launchData.FeaturesData;
            
            LaunchMinecraftProgress?.Invoke(LaunchProgress.Prepare);
            
            var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);

            var (minecraftData, minecraftDataError) = await _installerData.ReadMinecraftData(minecraftPaths, versionId,
                cancellationToken);

            if (minecraftData == null)
                return minecraftDataError;
            
            var launchFiles = _installerData.GetLaunchFiles(versionId, minecraftData, minecraftPaths);

            var launchArgumentsData = new LaunchArgumentsData(minecraftData, launchFiles, minecraftPaths, versionId, 
                playerName, featuresData.UseCustomResolution, featuresData.ScreenWidth, featuresData.ScreenHeight);
            
            if (!launchArgumentsData.IsValid)
                return ErrorCode.LaunchArgument;

            var launchArguments = VanillaArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);

            LaunchMinecraftProgress?.Invoke(LaunchProgress.StartGame);
            
            Logger.Log(launchArguments);

            if (!FileManager.FileExist(launchArgumentsData.JavaFile))
                return ErrorCode.JavaNotInstalled;
            
            var startGame = await FileManager.StartProcess(launchArgumentsData.JavaFile, launchArguments, startedAction,
                exitedAction);
            
            if (!startGame)
                return ErrorCode.StartProcess;
            
            LaunchMinecraftProgress?.Invoke(LaunchProgress.End);
            
            return ErrorCode.NoError;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            LaunchMinecraftProgress?.Invoke(LaunchProgress.End);
            return ErrorCode.GameAborted;
        }
    }
}