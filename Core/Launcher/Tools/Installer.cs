using JsonReader;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Forge;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.Data;
using Launcher.PublicData;
using ForgeVersion = Launcher.PublicData.ForgeVersion;

namespace Launcher.Tools;

internal sealed class Installer
{
    private readonly JsonManager _jsonManager;
    private readonly VersionsLoader _versionsLoader;

    public Installer(JsonManager jsonManager, VersionsLoader versionsLoader)
    {
        _jsonManager = jsonManager;
        _versionsLoader = versionsLoader;
    }

    public event Action<DownloadProgress>? DownloadingProgress;

    public async Task<ErrorCode> DownloadAndInstall(LaunchData launchData,
        MinecraftMissedInfo? minecraftMissedInfo = null, CancellationToken cancellationToken = default)
    {
        if (minecraftMissedInfo != null)
            return await DownloadAndInstallInternal(minecraftMissedInfo, cancellationToken);
        
        var versionId = launchData.ForgeVersionId ?? launchData.VersionId;
        
        if (!_versionsLoader.TryGetVersion(launchData.VersionId, out var version))
            return ErrorCode.GetVersionData;
        
        ForgeVersion? forgeVersion = null;
        if (launchData.ForgeVersionId != null)
        {
            forgeVersion = await _versionsLoader.GetForgeVersion(launchData.VersionId, launchData.ForgeVersionId,
                cancellationToken);
                
            if (forgeVersion == null)
                return ErrorCode.GetForgeVersionData;
        }
            
        return await DownloadAndInstallInternal(versionId, launchData.GameDirectory, version.Url,
            forgeVersion, cancellationToken);
    }

    private async Task<ErrorCode> DownloadAndInstallInternal(MinecraftMissedInfo minecraftMissedInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (minecraftMissedInfo.IsEmpty)
                return ErrorCode.NoError;
            
            var restoreResult = await RestoreMissedItems(minecraftMissedInfo, cancellationToken);
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

    private async Task<ErrorCode> DownloadAndInstallInternal(string versionId, string gameDirectory,
        string? minecraftUrl, ForgeVersion? forgeVersion = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(versionId))
                return ErrorCode.VersionId;
            
            var minecraftPaths = new MinecraftPaths(gameDirectory, versionId);

            var (minecraftData, minecraftDataError) = await GetAndSaveMinecraftData(minecraftUrl, versionId,
                minecraftPaths, cancellationToken);

            if (minecraftData == null)
                return minecraftDataError;

            var (assetsData, assetsDataError) =
                await GetAndSaveAssetsData(minecraftData, minecraftPaths, cancellationToken);

            if (assetsData == null)
                return assetsDataError;

            var (runtimeFiles, runtimeFilesError) = 
                await GetAndSaveRuntimes(minecraftData, minecraftPaths, cancellationToken);

            if (runtimeFiles == null)
                return runtimeFilesError;

            ForgeInfo? forgeInfo = null;
            if (forgeVersion != null)
            {
                var (forge, forgeInfoError) = await GetAndSaveForge(forgeVersion, minecraftPaths, cancellationToken);

                if (forge == null)
                    return forgeInfoError;

                forgeInfo = forge;
            }

            var fileList = FileManager.GetFileList(versionId, minecraftData, runtimeFiles, assetsData, minecraftPaths,
                forgeInfo);
            
            var missingInfoError = FileManager.GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
            if (missingInfoError != ErrorCode.NoError)
                return missingInfoError;
            
            if (!minecraftMissedInfo.IsEmpty)
            {
                var restoreResult = await RestoreMissedItems(minecraftMissedInfo, cancellationToken);
                if (restoreResult != ErrorCode.NoError)
                    return restoreResult;
            }

            return ErrorCode.NoError;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ErrorCode.Install;
        }
    }

    private async Task<(MinecraftData?, ErrorCode)> GetAndSaveMinecraftData(string? url, string versionId,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url))
            return (null, ErrorCode.Url);
            
        var minecraftVersionJson =
            await DownloadManager.DownloadJsonAsync(url, cancellationToken);

        if (string.IsNullOrEmpty(minecraftVersionJson))
            return (null, ErrorCode.Download);
            
        var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);
        
        if (minecraftData == null)
            return (null, ErrorCode.MinecraftData);
        
        var versionsDirectoryCreated = FileManager.CreateDirectory(minecraftPaths.VersionDirectory);
        if (!versionsDirectoryCreated)
            return (null, ErrorCode.CreateDirectory);
            
        var versionJsonCreated =
            await FileManager.WriteFile($"{minecraftPaths.VersionDirectory}\\{versionId}.json",
                minecraftVersionJson);
        
        if (!versionJsonCreated)
            return (null, ErrorCode.CreateFile);

        return (minecraftData, ErrorCode.NoError);
    }

    private async Task<(IReadOnlyList<Asset>?, ErrorCode)> GetAndSaveAssetsData(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var assetsJson = await DownloadManager.DownloadJsonAsync(minecraftData.AssetsIndex.Url, cancellationToken);
            
        if (string.IsNullOrEmpty(assetsJson))
            return (null, ErrorCode.Download);
            
        var assetsIndexDirectoryCreated = FileManager.CreateDirectory(minecraftPaths.AssetsIndexesDirectory);
        if (!assetsIndexDirectoryCreated)
            return (null, ErrorCode.CreateDirectory);

        var assetsIndexJsonCreated =
            await FileManager.WriteFile(
                $"{minecraftPaths.AssetsIndexesDirectory}\\{minecraftData.AssetsVersion}.json", assetsJson);
        
        if (!assetsIndexJsonCreated)
            return (null, ErrorCode.CreateFile);
            
        var assetsData = _jsonManager.GetAssets(assetsJson);
        if (assetsData == null)
            return (null, ErrorCode.AssetsData);

        return (assetsData, ErrorCode.NoError);
    }

    private async Task<(RuntimeFiles?, ErrorCode)> GetAndSaveRuntimes(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var allRuntimesJson = await DownloadManager.DownloadJsonAsync(WellKnownUrls.JavaRuntimesUrl,
            cancellationToken);
        
        if (string.IsNullOrEmpty(allRuntimesJson))
            return (null, ErrorCode.Download);

        var runtimes = _jsonManager.GetAllRuntimes(allRuntimesJson);

        if (runtimes == null)
            return (null, ErrorCode.RuntimeData);

        var osRuntime = OsRuleManager.GetOsRuntime(runtimes);
        
        if (osRuntime == null)
            return (null, ErrorCode.RuntimeData);

        var runtimeType = minecraftData.JavaVersion.Component;
        Runtime currentRuntime;
        switch (runtimeType)
        {
            case WellKnownRuntimeTypes.JavaRuntimeAlpha:
                if (osRuntime.JavaRuntimeAlpha == null)
                    return (null, ErrorCode.RuntimeDataNotFound);
                
                currentRuntime = osRuntime.JavaRuntimeAlpha;
                break;
            case WellKnownRuntimeTypes.JavaRuntimeBeta:
                if (osRuntime.JavaRuntimeBeta == null)
                    return (null, ErrorCode.RuntimeDataNotFound);
                
                currentRuntime = osRuntime.JavaRuntimeBeta;
                break;
            case WellKnownRuntimeTypes.JavaRuntimeGamma:
                if (osRuntime.JavaRuntimeGamma == null)
                    return (null, ErrorCode.RuntimeDataNotFound);
                
                currentRuntime = osRuntime.JavaRuntimeGamma;
                break;
            case WellKnownRuntimeTypes.JreLegacy:
                if (osRuntime.JreLegacy == null)
                    return (null, ErrorCode.RuntimeDataNotFound);
                
                currentRuntime = osRuntime.JreLegacy;
                break;
            case WellKnownRuntimeTypes.MinecraftJavaExe:
                if (osRuntime.MinecraftJavaExe == null)
                    return (null, ErrorCode.RuntimeDataNotFound);
                
                currentRuntime = osRuntime.MinecraftJavaExe;
                break;
            default:
                return (null, ErrorCode.UnknownRuntimeVersion);
        }
        
        var currentRuntimeJson = await DownloadManager.DownloadJsonAsync(currentRuntime.Url,
            cancellationToken);

        var runtimeFiles = _jsonManager.GetRuntimeFiles(currentRuntimeJson);
        if (runtimeFiles == null)
            return (null, ErrorCode.RuntimeData);
        
        var runtimeFilesJsonCreated = await FileManager.WriteFile(
            $"{minecraftPaths.RuntimeDirectory}\\{minecraftData.JavaVersion.Component}-{OsRuleManager.CurrentOsName}.json",
            currentRuntimeJson);
        
        if (!runtimeFilesJsonCreated)
            return (null, ErrorCode.CreateFile);

        return (runtimeFiles, ErrorCode.NoError);
    }

    private async Task<(ForgeInfo?, ErrorCode)> GetAndSaveForge(ForgeVersion forgeVersion,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var forgeJson = await DownloadManager.DownloadJsonAsync(forgeVersion.Url, cancellationToken);
        
        if (string.IsNullOrEmpty(forgeJson))
            return (null, ErrorCode.Download);

        var forgeInfo = _jsonManager.GetForgeInfo(forgeJson);

        if (forgeInfo == null)
            return (null, ErrorCode.ForgeData);
        
        var forgeInfoJsonCreated = await FileManager.WriteFile(
            $"{minecraftPaths.VersionDirectory}\\FORGE-{forgeVersion.Id}.json", forgeJson);
        
        if (!forgeInfoJsonCreated)
            return (null, ErrorCode.CreateFile);
        
        return (forgeInfo, ErrorCode.NoError);
    }

    private async Task<ErrorCode> RestoreMissedItems(MinecraftMissedInfo missedInfo,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < missedInfo.DirectoriesToCreate.Count; i++)
        {
            var result = FileManager.CreateDirectory(missedInfo.DirectoriesToCreate[i]);
            if (!result)
                return ErrorCode.CreateDirectory;
        }

        for (var i = 0; i < missedInfo.CorruptedFiles.Count; i++)
        {
            var result = FileManager.Delete(missedInfo.CorruptedFiles[i]);
            if (!result)
                return ErrorCode.DeleteFileOrDirectory;
        }

        if (missedInfo.DownloadQueue.Count > 0)
        {
            var result = await DownloadMissingFiles(missedInfo.DownloadQueue, missedInfo.DownloadQueue.Count,
                missedInfo.TotalDownloadSize, cancellationToken);
            if (!result)
                return ErrorCode.Download;
        }

        for (var i = 0; i < missedInfo.UnpackItems.Count; i++)
        {
            var (fileName, destination) = missedInfo.UnpackItems[i];
            var result = FileManager.ExtractToDirectory(fileName, destination);
            if (!result)
                return ErrorCode.ExtractArchive;
        }

        for (var i = 0; i < missedInfo.PathsToDelete.Count; i++)
        {
            var result = FileManager.Delete(missedInfo.PathsToDelete[i]);
            if (!result)
                return ErrorCode.DeleteFileOrDirectory;
        }

        return ErrorCode.NoError;
    }

    private async Task<bool> DownloadMissingFiles(IEnumerable<(Uri source, string fileName)> downloadQueue, 
        int totalFilesCount, long totalSize, CancellationToken cancellationToken)
    {
        var downloadedCount = 0;
        
        DownloadingProgress?.Invoke(new DownloadProgress(0, totalSize, 0, totalFilesCount));
        return await DownloadManager.DownloadFilesParallel(downloadQueue, cancellationToken, BytesReceived,
            FileDownloaded);
        
        void BytesReceived(long bytesReceived)
        {
            DownloadingProgress?.Invoke(
                new DownloadProgress(bytesReceived, totalSize, downloadedCount, totalFilesCount));
        }

        void FileDownloaded()
        {
            downloadedCount++;
        }
    }
}