using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Forge;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.Data;
using Launcher.PublicData;
using Launcher.Tools;
using ForgeVersion = Launcher.PublicData.ForgeVersion;

namespace Launcher.Interfaces;

internal interface IForgeInstallerData : IInstallerData
{
    Task<(ForgeInfo?, ErrorCode)> GetAndSaveForge(ForgeVersion forgeVersion,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken);

    MinecraftFileList GetForgeFileList(string versionId, MinecraftData data, ForgeInfo forgeInfo,
        RuntimeFiles runtimeFiles, IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths);

    Task<(ForgeInfo?, ErrorCode)> ReadForgeInfo(string forgeVersionId, MinecraftPaths minecraftPaths, 
        CancellationToken cancellationToken);

    MinecraftLaunchFiles GetForgeLaunchFiles(string versionId, MinecraftData data, ForgeInfo forgeInfo,
        MinecraftPaths minecraftPaths);
}