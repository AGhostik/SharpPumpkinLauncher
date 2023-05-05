using JsonReader;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Forge;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.Data;
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
    
    public async Task<MinecraftMissedInfo?> IsVersionInstalled(LaunchData launchData,
        CancellationToken cancellationToken)
    {
        var versionId = launchData.ForgeVersionId ?? launchData.VersionId;
        
        var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, versionId);
        
        var (minecraftData, _) = await ReadMinecraftData(minecraftPaths, versionId, cancellationToken);

        if (minecraftData == null)
            return null;

        var assetsData = await ReadAssetsData(minecraftData, minecraftPaths, cancellationToken);

        if (assetsData == null)
            return null;

        var runtimeFiles = await ReadRuntimesData(minecraftData, minecraftPaths, cancellationToken);

        if (runtimeFiles == null)
            return null;
        
        ForgeInfo? forgeInfo = null;
        if (launchData.ForgeVersionId != null)
        {
            var (forge, _) = await ReadForgeInfo(launchData.ForgeVersionId, minecraftPaths, cancellationToken);

            if (forge == null)
                return null;

            forgeInfo = forge;
        }
        
        var fileList = FileManager.GetFileList(versionId, minecraftData, runtimeFiles, assetsData, minecraftPaths,
            forgeInfo);
            
        var missingInfoError = FileManager.GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
        if (missingInfoError != ErrorCode.NoError)
            return null;

        return minecraftMissedInfo;
    }

    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, Action? startedAction = null,
        Action? exitedAction = null, CancellationToken cancellationToken = default)
    {
        var versionId = launchData.ForgeVersionId ?? launchData.VersionId;
        
        return await LaunchMinecraftInternal(launchData.GameDirectory, versionId, launchData.PlayerName,
            launchData.ForgeVersionId, launchData.UseCustomResolution, launchData.ScreenWidth, launchData.ScreenHeight,
            startedAction, exitedAction, cancellationToken);
    }
    
    private async Task<ErrorCode> LaunchMinecraftInternal(string gameDirectory, string versionId, string playerName, 
        string? forgeVersionId = null, bool useCustomResolution = false, int screenWidth = 0, int screenHeight = 0,
        Action? startedAction = null, Action? exitedAction = null, CancellationToken cancellationToken = default)
    {
        try
        {
            LaunchMinecraftProgress?.Invoke(LaunchProgress.Prepare);
            
            var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);

            var (minecraftData, minecraftDataError) = await ReadMinecraftData(minecraftPaths, versionId,
                cancellationToken);

            if (minecraftData == null)
                return minecraftDataError;
            
            ForgeInfo? forgeInfo = null;
            if (forgeVersionId != null)
            {
                var (forge, forgeInfoError) =
                    await ReadForgeInfo(forgeVersionId, minecraftPaths, cancellationToken);

                if (forge == null)
                    return forgeInfoError;

                forgeInfo = forge;
            }

            var launchFiles = FileManager.GetLaunchFiles(versionId, minecraftData, minecraftPaths, forgeInfo);

            var launchArgumentsData = new LaunchArgumentsData(minecraftData, launchFiles, minecraftPaths,
                versionId, playerName, useCustomResolution, screenWidth, screenHeight);
            
            if (!launchArgumentsData.IsValid)
                return ErrorCode.LaunchArgument;

            string launchArguments;
            if (forgeInfo != null)
            {
                launchArguments =
                    LaunchArgumentsBuilder.GetForgeLaunchArguments(forgeInfo, minecraftData, launchArgumentsData);
            }
            else
            {
                launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);
            }

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
    
    private async Task<IReadOnlyList<Asset>?> ReadAssetsData(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var assetsDataJson = await FileManager.ReadFile(
            $"{minecraftPaths.AssetsIndexesDirectory}\\{minecraftData.AssetsVersion}.json",
            cancellationToken);

        if (string.IsNullOrEmpty(assetsDataJson))
            return null;
            
        var assetsData = _jsonManager.GetAssets(assetsDataJson);

        return assetsData;
    }
    
    private async Task<RuntimeFiles?> ReadRuntimesData(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var runtimesDataJson = await FileManager.ReadFile(
            $"{minecraftPaths.RuntimeDirectory}\\{minecraftData.JavaVersion.Component}-{OsRuleManager.CurrentOsName}.json",
            cancellationToken);

        if (string.IsNullOrEmpty(runtimesDataJson))
            return null;
            
        var runtimeFiles = _jsonManager.GetRuntimeFiles(runtimesDataJson);

        return runtimeFiles;
    }
    
    private async Task<(ForgeInfo?, ErrorCode)> ReadForgeInfo(string forgeVersionId,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var forgeInfoJson =
            await FileManager.ReadFile($"{minecraftPaths.VersionDirectory}\\FORGE-{forgeVersionId}.json",
                cancellationToken);

        if (string.IsNullOrEmpty(forgeInfoJson))
            return (null, ErrorCode.ReadFile);
            
        var forgeInfo = _jsonManager.GetForgeInfo(forgeInfoJson);

        if (forgeInfo == null)
            return (null, ErrorCode.ForgeData);

        return (forgeInfo, ErrorCode.NoError);
    }
}