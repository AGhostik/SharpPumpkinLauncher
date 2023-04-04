using JsonReader;
using JsonReader.PublicData.Manifest;
using Launcher.Data;
using Launcher.PublicData;
using Launcher.Tools;
using SimpleLogger;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

internal sealed class OnlineLauncher : ILauncher
{
    private readonly JsonManager _jsonManager;
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;

    public OnlineLauncher()
    {
        _jsonManager = new JsonManager();
    }
    
    public async Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken)
    {
        var versionsJson = await DownloadManager.DownloadJsonAsync(WellKnownUrls.VersionsUrl, cancellationToken);
        var versions = _jsonManager.GetVersions(versionsJson);
        
        if (versions == null)
            return Versions.Empty;

        return new Versions(
            versions.Latest,
            versions.LatestSnapshot,
            GetVersionList(versions.Release), 
            GetVersionList(versions.Snapshot), 
            GetVersionList(versions.Beta), 
            GetVersionList(versions.Alpha));
    }
    
    public async Task<ErrorCode> LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken,
        Action? exitedAction = null)
    {
        try
        {
            if (string.IsNullOrEmpty(launchData.Version.Id))
                return ErrorCode.VersionId;
            
            if (string.IsNullOrEmpty(launchData.Version.Url))
                return ErrorCode.Url;

            LaunchMinecraftProgress?.Invoke(LaunchProgress.Prepare, 0f);

            var minecraftVersionJson =
                await DownloadManager.DownloadJsonAsync(launchData.Version.Url, cancellationToken);

            if (string.IsNullOrEmpty(minecraftVersionJson))
                return ErrorCode.Download;
            
            var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

            if (minecraftData == null)
                return ErrorCode.MinecraftData;

            var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, minecraftData.Id);
            
            var versionsDirectoryCreated = FileManager.CreateDirectory(minecraftPaths.VersionDirectory);
            if (!versionsDirectoryCreated)
                return ErrorCode.CreateDirectory;
            
            var versionJsonCreated =
                await FileManager.WriteFile($"{minecraftPaths.VersionDirectory}\\{minecraftData.Id}.json",
                    minecraftVersionJson);
            if (!versionJsonCreated)
                return ErrorCode.CreateFile;

            var assetsJson = await DownloadManager.DownloadJsonAsync(minecraftData.AssetsIndex.Url, cancellationToken);
            
            if (string.IsNullOrEmpty(assetsJson))
                return ErrorCode.Download;
            
            var assetsIndexDirectoryCreated = FileManager.CreateDirectory(minecraftPaths.AssetsIndexesDirectory);
            if (!assetsIndexDirectoryCreated)
                return ErrorCode.CreateDirectory;

            var assetsIndexJsonCreated =
                await FileManager.WriteFile(
                    $"{minecraftPaths.AssetsIndexesDirectory}\\{minecraftData.AssetsVersion}.json", assetsJson);
            if (!assetsIndexJsonCreated)
                return ErrorCode.CreateFile;
            
            var assetsData = _jsonManager.GetAssets(assetsJson);
            if (assetsData == null)
                return ErrorCode.AssetsData;

            var fileList = FileManager.GetFileList(minecraftData, assetsData, minecraftPaths, minecraftData.Id);

            var launchArgumentsData =
                new LaunchArgumentsData(minecraftData, fileList, minecraftPaths, launchData.PlayerName);

            if (!launchArgumentsData.IsValid)
                return ErrorCode.LaunchArgument;
            
            var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);
            
            Logger.Log(launchArguments.Replace(" ", Environment.NewLine));

            var missingInfoError = GetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo);
            if (missingInfoError != ErrorCode.NoError)
                return missingInfoError;
            
            if (!minecraftMissedInfo.IsEmpty)
            {
                var restoreResult = await RestoreMissedItems(minecraftMissedInfo, cancellationToken);
                if (restoreResult != ErrorCode.NoError)
                    return restoreResult;
            }

            LaunchMinecraftProgress?.Invoke(LaunchProgress.StartGame, 0f);

            var startGame = await FileManager.StartProcess("java", launchArguments, exitedAction);
            if (!startGame)
                return ErrorCode.StartProcess;
            
            LaunchMinecraftProgress?.Invoke(LaunchProgress.End, 0f);
            
            return ErrorCode.NoError;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            LaunchMinecraftProgress?.Invoke(LaunchProgress.End, 0f);
            return ErrorCode.GameAborted;
        }
    }

    private async Task<ErrorCode> RestoreMissedItems(MinecraftMissedInfo missedInfo, CancellationToken cancellationToken)
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
            var result = await DownloadMissingFiles(missedInfo.DownloadQueue, missedInfo.TotalDownloadSize,
                cancellationToken);
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

    private async Task<bool> DownloadMissingFiles(IEnumerable<(Uri source, string fileName)> downloadQueue, long totalSize,
        CancellationToken cancellationToken)
    {
        LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, 0f);
        return await DownloadManager.DownloadFilesParallel(downloadQueue, cancellationToken, Callback);
        
        void Callback(long bytesReceived)
        {
            LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, (float)bytesReceived / totalSize);
        }
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

        return ErrorCode.NoError;
    }

    private static ErrorCode CheckFileAndDirectoryMissed(ref MinecraftMissedInfo missedInfo, IMinecraftFile minecraftFile)
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

    private static List<PublicData.Version> GetVersionList(IEnumerable<MinecraftVersion> minecraftVersions)
    {
        var result = new List<PublicData.Version>();
        foreach (var minecraftVersion in minecraftVersions)
        {
            try
            {
                var type = MinecraftTypeConverter.GetVersionType(minecraftVersion.Type);
                var version = new PublicData.Version(minecraftVersion.Id, minecraftVersion.Url, type);
                
                result.Add(version);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Log(e);
            }
        }

        return result;
    }
}