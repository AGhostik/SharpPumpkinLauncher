using JsonReader;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.PublicData;

namespace Launcher.Tools;

internal sealed class GameLauncher
{
    private readonly JsonManager _jsonManager;

    public GameLauncher(JsonManager jsonManager)
    {
        _jsonManager = jsonManager;
    }
    
    public event Action<LaunchProgress>? LaunchMinecraftProgress;

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken,
        Action? startedAction = null, Action? exitedAction = null)
    {
        try
        {
            LaunchMinecraftProgress?.Invoke(LaunchProgress.Prepare);
            
            var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, launchData.Version.Id);

            var (minecraftData, minecraftDataError) = await ReadMinecraftData(minecraftPaths, launchData.Version.Id,
                cancellationToken);

            if (minecraftData == null)
                return minecraftDataError;

            var launchFiles = FileManager.GetLaunchFiles(minecraftData, minecraftPaths);

            var launchArgumentsData =
                new LaunchArgumentsData(minecraftData, launchFiles, minecraftPaths, launchData.PlayerName);
            
            if (!launchArgumentsData.IsValid)
                return ErrorCode.LaunchArgument;
            
            var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);

            LaunchMinecraftProgress?.Invoke(LaunchProgress.StartGame);

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

    private async Task<(MinecraftData?, ErrorCode)> ReadMinecraftData(MinecraftPaths minecraftPaths, string versionId,
        CancellationToken cancellationToken)
    {
        var minecraftVersionJson =
            await FileManager.ReadFile($"{minecraftPaths.VersionDirectory}\\{versionId}.json",
                cancellationToken);

        if (string.IsNullOrEmpty(minecraftVersionJson))
            return (null, ErrorCode.ReadFile);

        var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

        if (minecraftData == null)
            return (null, ErrorCode.MinecraftData);

        return (minecraftData, ErrorCode.NoError);
    }
}