using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.Data;
using Launcher.PublicData;
using Launcher.Tools;

namespace Launcher.Interfaces;

internal interface IInstallerData
{
    Task<(MinecraftData?, ErrorCode)> GetAndSaveMinecraftData(string? url, string versionId,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Asset>?, ErrorCode)> GetAndSaveAssetsData(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken);

    Task<(RuntimeFiles?, ErrorCode)> GetAndSaveRuntimes(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken);

    Task<(MinecraftData?, ErrorCode)> ReadMinecraftData(MinecraftPaths minecraftPaths, string versionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Asset>?> ReadAssetsData(MinecraftData minecraftData, MinecraftPaths minecraftPaths, 
        CancellationToken cancellationToken);

    Task<RuntimeFiles?> ReadRuntimesData(MinecraftData minecraftData, MinecraftPaths minecraftPaths,
        CancellationToken cancellationToken);

    MinecraftFileList GetFileList(string versionId, MinecraftData data, RuntimeFiles runtimeFiles,
        IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths);

    (MinecraftMissedInfo?, ErrorCode) GetMissingInfo(IMinecraftFileList minecraftFileList, 
        MinecraftPaths minecraftPaths);

    MinecraftLaunchFiles GetLaunchFiles(string versionId, MinecraftData data, MinecraftPaths minecraftPaths);
}