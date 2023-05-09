using Launcher.Data;
using Launcher.Interfaces;
using Launcher.PublicData;
using Launcher.Tools;

namespace Launcher.Vanilla;

internal sealed class VanillaInstaller : IInstaller
{
    private readonly BaseInstaller _baseInstaller;
    private readonly VanillaVersionsLoader _vanillaVersionsLoader;
    private readonly IInstallerData _installerData;

    public VanillaInstaller(BaseInstaller baseInstaller, VanillaVersionsLoader vanillaVersionsLoader,
        IInstallerData installerData)
    {
        _baseInstaller = baseInstaller;
        _vanillaVersionsLoader = vanillaVersionsLoader;
        _installerData = installerData;

        _baseInstaller.DownloadingProgress += progress => DownloadingProgress?.Invoke(progress);
    }

    public event Action<DownloadProgress>? DownloadingProgress;
    
    public async Task<MinecraftMissedInfo?> IsVersionInstalled(LaunchData launchData,
        CancellationToken cancellationToken)
    {
        var versionId = launchData.VersionId;
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
        
        var fileList = _installerData.GetFileList(versionId, minecraftData, runtimeFiles, assetsData, minecraftPaths);
            
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

        var (missedInfo, missedInfoError) = await GetMinecraftMissedInfo(versionId, gameDirectory, version.Url, 
            cancellationToken);

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
        string? minecraftUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(versionId))
            return (null, ErrorCode.VersionId);
        
        var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);

        var (minecraftData, minecraftDataError) = await _installerData.GetAndSaveMinecraftData(minecraftUrl, versionId,
            minecraftPaths, cancellationToken);

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

        var fileList = _installerData.GetFileList(versionId, minecraftData, runtimeFiles, assetsData, minecraftPaths);
        
        var missingInfoError = _installerData.GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
        if (missingInfoError != ErrorCode.NoError)
            return (null, missingInfoError);

        return (minecraftMissedInfo, ErrorCode.NoError);
    }
}