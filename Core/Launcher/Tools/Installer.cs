using JsonReader;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.Data;
using Launcher.PublicData;
using SimpleLogger;

namespace Launcher.Tools;

internal sealed class Installer
{
    private readonly JsonManager _jsonManager;

    public Installer(JsonManager jsonManager)
    {
        _jsonManager = jsonManager;
    }

    public event Action<DownloadProgress>? DownloadingProgress;
    
    public async Task<ErrorCode> DownloadAndInstall(LaunchData launchData, CancellationToken cancellationToken)
    {
        try
        {
            if (!await DownloadManager.CheckConnection(cancellationToken))
                return ErrorCode.Connection;
            
            if (string.IsNullOrEmpty(launchData.Version.Id))
                return ErrorCode.VersionId;
            
            var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, launchData.Version.Id);

            var (minecraftData, minecraftDataError) = await GetAndSaveMinecraftData(launchData.Version.Url,
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
            
            var runtimeType = minecraftData.JavaVersion.Component;
            var fileList = FileManager.GetFileList(runtimeFiles, runtimeType, minecraftData, assetsData, minecraftPaths);
            
            var missingInfoError = GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
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

    private async Task<(MinecraftData?, ErrorCode)> GetAndSaveMinecraftData(string? url, MinecraftPaths minecraftPaths,
        CancellationToken cancellationToken)
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
            await FileManager.WriteFile($"{minecraftPaths.VersionDirectory}\\{minecraftData.Id}.json",
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
    
    private static ErrorCode GetMissingInfo(MinecraftFileList minecraftFileList, MinecraftPaths minecraftPaths,
        out MinecraftMissedInfo minecraftMissedInfo)
    {
        minecraftMissedInfo = new MinecraftMissedInfo();
        
        var clientFileError = CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Client);
        if (clientFileError != ErrorCode.NoError)
            return clientFileError;

        if (minecraftFileList.Server != null)
        {
            var serverFileError = CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Server);
            if (serverFileError != ErrorCode.NoError)
                return serverFileError;
        }

        if (minecraftFileList.Logging != null)
        {
            var loggingFileError = CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Logging);
            if (loggingFileError != ErrorCode.NoError)
                return loggingFileError;
        }

        for (var i = 0; i < minecraftFileList.LibraryFiles.Count; i++)
        {
            var libraryFile = minecraftFileList.LibraryFiles[i];
            var libraryFileError = CheckFileAndDirectoryMissed(ref minecraftMissedInfo, libraryFile);
            if (libraryFileError != ErrorCode.NoError)
                return libraryFileError;

            if (libraryFile.NeedUnpack)
            {
                var natives = minecraftPaths.NativesDirectory;
                if (!FileManager.DirectoryExist(natives) && !minecraftMissedInfo.DirectoriesToCreate.Contains(natives))
                    minecraftMissedInfo.DirectoriesToCreate.Add(natives);

                minecraftMissedInfo.UnpackItems.Add((libraryFile.FileName, natives));
            }

            if (libraryFile.Delete != null)
            {
                var unpackDirectory = minecraftPaths.NativesDirectory;
                for (var j = 0; j < libraryFile.Delete.Count; j++)
                {
                    var path = $"{unpackDirectory}\\{libraryFile.Delete[j]}";
                    minecraftMissedInfo.PathsToDelete.Add(path);
                }
            }
        }

        for (var i = 0; i < minecraftFileList.AssetFiles.Count; i++)
        {
            var assetFileError = CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.AssetFiles[i]);
            if (assetFileError != ErrorCode.NoError)
                return assetFileError;
        }

        for (var i = 0; i < minecraftFileList.JavaRuntimeFiles.Count; i++)
        {
            var javaRuntimeFileError =
                CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.JavaRuntimeFiles[i]);
            if (javaRuntimeFileError != ErrorCode.NoError)
                return javaRuntimeFileError;
        }

        return ErrorCode.NoError;
    }

    private static ErrorCode CheckFileAndDirectoryMissed(ref MinecraftMissedInfo missedInfo,
        IMinecraftFile minecraftFile)
    {
        if (!FileManager.FileExist(minecraftFile.FileName))
        {
            missedInfo.DownloadQueue.Add((new Uri(minecraftFile.Url), minecraftFile.FileName));
            missedInfo.TotalDownloadSize += minecraftFile.Size;
        }
        else
        {
            var sha1 = FileManager.ComputeSha1(minecraftFile.FileName);
            if (string.IsNullOrEmpty(sha1))
            {
                Logger.Log($"Cant compute sha1 for file: {minecraftFile.FileName}");
            }
            else if (sha1 != minecraftFile.Sha1)
            {
                Logger.Log($"File {minecraftFile.FileName} corrupted ({sha1} != {minecraftFile.Sha1})");
                missedInfo.CorruptedFiles.Add(minecraftFile.FileName);
                
                missedInfo.DownloadQueue.Add((new Uri(minecraftFile.Url), minecraftFile.FileName));
                missedInfo.TotalDownloadSize += minecraftFile.Size;
            }
        }

        var directory = FileManager.GetPathDirectory(minecraftFile.FileName);
        if (!string.IsNullOrEmpty(directory))
        {
            if (!FileManager.DirectoryExist(directory) && !missedInfo.DirectoriesToCreate.Contains(directory))
                missedInfo.DirectoriesToCreate.Add(directory);
        }
        else
        {
            return ErrorCode.Check;
        }

        return ErrorCode.NoError;
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