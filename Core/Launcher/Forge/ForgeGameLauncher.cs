using Launcher.Interfaces;
using Launcher.PublicData;
using Launcher.Tools;
using SimpleLogger;

namespace Launcher.Forge;

internal sealed class ForgeGameLauncher : IGameLauncher
{
    private readonly IForgeInstallerData _installerData;

    public ForgeGameLauncher(IForgeInstallerData forgeInstallerData)
    {
        _installerData = forgeInstallerData;
    }
    
    public event Action<LaunchProgress>? LaunchMinecraftProgress;
    
    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, Action? startedAction = null,
        Action? exitedAction = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var versionId = launchData.ForgeVersionId;

            if (string.IsNullOrEmpty(versionId))
                return ErrorCode.VersionId;
            
            var gameDirectory = launchData.GameDirectory;
            var playerName = launchData.PlayerName;
            var featuresData = launchData.FeaturesData;
            
            LaunchMinecraftProgress?.Invoke(LaunchProgress.Prepare);
            
            var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);

            var (minecraftData, minecraftDataError) = await _installerData.ReadMinecraftData(minecraftPaths, versionId,
                cancellationToken);

            if (minecraftData == null)
                return minecraftDataError;
            
            var (forgeInfo, forgeInfoError) =
                await _installerData.ReadForgeInfo(versionId, minecraftPaths, cancellationToken);

            if (forgeInfo == null)
                return forgeInfoError;

            var launchFiles = _installerData.GetForgeLaunchFiles(versionId, minecraftData, forgeInfo, minecraftPaths);

            var launchArgumentsData = new LaunchArgumentsData(minecraftData, launchFiles, minecraftPaths, versionId,
                playerName, featuresData.UseCustomResolution, featuresData.ScreenWidth, featuresData.ScreenHeight);
            
            if (!launchArgumentsData.IsValid)
                return ErrorCode.LaunchArgument;

            var launchArguments =
                ForgeArgumentsBuilder.GetForgeLaunchArguments(forgeInfo, minecraftData, launchArgumentsData);

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