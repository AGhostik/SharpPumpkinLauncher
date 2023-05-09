using Launcher.Data;
using Launcher.Interfaces;
using Launcher.PublicData;
using Launcher.Tools;
using Launcher.Vanilla;
using ForgeVersion = Launcher.PublicData.ForgeVersion;

namespace Launcher.Forge;

internal sealed class ForgeInstaller : IInstaller
{
    private readonly BaseInstaller _baseInstaller;
    private readonly VanillaVersionsLoader _vanillaVersionsLoader;
    private readonly ForgeVersionsLoader _forgeVersionsLoader;
    private readonly IForgeInstallerData _installerData;
    private readonly ForgeProfileInstaller _forgeProfileInstaller;

    public ForgeInstaller(BaseInstaller baseInstaller, VanillaVersionsLoader vanillaVersionsLoader,
        ForgeVersionsLoader forgeVersionsLoader, IForgeInstallerData installerData)
    {
        _baseInstaller = baseInstaller;
        _vanillaVersionsLoader = vanillaVersionsLoader;
        _forgeVersionsLoader = forgeVersionsLoader;
        _installerData = installerData;

        _forgeProfileInstaller = new ForgeProfileInstaller();
        
        _baseInstaller.DownloadingProgress += progress => DownloadingProgress?.Invoke(progress);
    }

    public event Action<DownloadProgress>? DownloadingProgress;
    
    public async Task<MinecraftMissedInfo?> IsVersionInstalled(LaunchData launchData, 
        CancellationToken cancellationToken)
    {
        var versionId = launchData.ForgeVersionId;

        if (string.IsNullOrEmpty(versionId))
            return null;
            
        var gameDirectory = launchData.GameDirectory;
        
        var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);
        
        var (minecraftData, _) = await _installerData.ReadMinecraftData(minecraftPaths, versionId, cancellationToken);

        if (minecraftData == null)
            return null;

        var assetsData = await _installerData.ReadAssetsData(minecraftData, minecraftPaths, cancellationToken);

        if (assetsData == null)
            return null;

        var runtimeFiles = await _installerData.ReadRuntimesData(minecraftData, minecraftPaths, cancellationToken);

        if (runtimeFiles == null)
            return null;
        
        var (forgeInfo, _) = await _installerData.ReadForgeInfo(versionId, minecraftPaths, cancellationToken);

        if (forgeInfo == null)
            return null;
        
        var fileList = _installerData.GetForgeFileList(versionId, minecraftData, forgeInfo, runtimeFiles, assetsData, 
            minecraftPaths);
            
        var missingInfoError = _installerData.GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
        if (missingInfoError != ErrorCode.NoError)
            return null;

        return minecraftMissedInfo;
    }
    
    public async Task<ErrorCode> DownloadAndInstall(string versionId, string gameDirectory,
        MinecraftMissedInfo? minecraftMissedInfo = null, CancellationToken cancellationToken = default)
    {
        if (minecraftMissedInfo != null)
            return await DownloadAndInstallInternal(minecraftMissedInfo, cancellationToken:cancellationToken);
        
        if (!_vanillaVersionsLoader.TryGetVersion(versionId, out var version))
            return ErrorCode.GetVersionData;
        
        if (string.IsNullOrEmpty(version.Url))
            return ErrorCode.Url;

        if (string.IsNullOrEmpty(versionId))
            return ErrorCode.VersionId;

        var forgeVersion = 
            await _forgeVersionsLoader.GetForgeVersion(versionId, versionId, cancellationToken);
        
        if (forgeVersion == null)
            return ErrorCode.GetForgeVersionData;

        var (missedInfo, missedInfoError) = await GetMinecraftMissedInfo(versionId, gameDirectory, version.Url, 
            forgeVersion, cancellationToken);

        if (missedInfo == null)
            return missedInfoError;
        
        return await DownloadAndInstallInternal(missedInfo, cancellationToken);
    }

    private async Task<ErrorCode> DownloadAndInstallInternal(MinecraftMissedInfo minecraftMissedInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (minecraftMissedInfo.IsEmpty)
                return ErrorCode.NoError;
            
            var restoreResult = await _baseInstaller.RestoreMissedItems(minecraftMissedInfo, cancellationToken);
            
            if (restoreResult != ErrorCode.NoError)
                return restoreResult;

            return ErrorCode.NoError;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ErrorCode.Install;
        }
    }
    
    private async Task<(MinecraftMissedInfo?, ErrorCode)> GetMinecraftMissedInfo(string versionId, string gameDirectory,
        string minecraftUrl, ForgeVersion forgeVersion, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(versionId))
            return (null, ErrorCode.VersionId);
        
        var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);

        var (minecraftData, minecraftDataError) = 
            await _installerData.GetAndSaveMinecraftData(minecraftUrl, versionId, minecraftPaths, cancellationToken);

        if (minecraftData == null)
            return (null, minecraftDataError);

        var (assetsData, assetsDataError) =
            await _installerData.GetAndSaveAssetsData(minecraftData, minecraftPaths, cancellationToken);

        if (assetsData == null)
            return (null, assetsDataError);

        var (runtimeFiles, runtimeFilesError) = 
            await _installerData.GetAndSaveRuntimes(minecraftData, minecraftPaths, cancellationToken);

        if (runtimeFiles == null)
            return (null, runtimeFilesError);

        var (forgeInfo, forgeInfoError) = 
            await _installerData.GetAndSaveForge(forgeVersion, minecraftPaths, cancellationToken);

        if (forgeInfo == null)
            return (null, forgeInfoError);

        var fileList = _installerData.GetForgeFileList(versionId, minecraftData, forgeInfo, runtimeFiles, assetsData, 
            minecraftPaths);
        
        var missingInfoError = _installerData.GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
        if (missingInfoError != ErrorCode.NoError)
            return (null, missingInfoError);
        
        var javaFile =
            $"{minecraftPaths.RuntimeDirectory}\\{minecraftData.JavaVersion.Component}\\{OsRuleManager.GetJavaExecutablePath()}";
        
        minecraftMissedInfo.AfterInstallTask = _forgeProfileInstaller.Install(forgeInfo, javaFile,
            $"{minecraftPaths.VersionDirectory}\\{versionId}.jar", minecraftPaths.LibrariesDirectory);

        return (minecraftMissedInfo, ErrorCode.NoError);
    }
}