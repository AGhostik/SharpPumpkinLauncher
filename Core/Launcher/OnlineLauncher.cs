using System.Diagnostics;
using JsonReader;
using JsonReader.PublicData.Manifest;
using Launcher.Data;
using Launcher.PublicData;
using Launcher.Tools;
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
    
    public async Task LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken,
        Action? exitedAction = null)
    {
        try
        {
            if (string.IsNullOrEmpty(launchData.Version.Id))
                return;
            
            if (string.IsNullOrEmpty(launchData.Version.Url))
                return;

            LaunchMinecraftProgress?.Invoke(LaunchProgress.GetVersionData, 0f);

            var minecraftVersionJson = await DownloadManager.DownloadJsonAsync(launchData.Version.Url, cancellationToken);
            var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

            if (minecraftData == null)
                return;

            var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, minecraftData.Id);
            
            FileManager.CreateDirectory(minecraftPaths.VersionDirectory);
            await FileManager.WriteFile($"{minecraftPaths.VersionDirectory}\\{minecraftData.Id}.json",
                minecraftVersionJson);

            var assetsJson = await DownloadManager.DownloadJsonAsync(minecraftData.AssetsIndex.Url, cancellationToken);
            FileManager.CreateDirectory(minecraftPaths.AssetsIndexDirectory);
            await FileManager.WriteFile(
                $"{minecraftPaths.AssetsIndexDirectory}\\{minecraftData.AssetsVersion}.json",
                assetsJson);
            
            var assetsData = _jsonManager.GetAssets(assetsJson);
            if (assetsData == null)
                return;

            var fileList = FileManager.GetFileList(minecraftData, assetsData, minecraftPaths, minecraftData.Id);

            var launchArgumentsData =
                new LaunchArgumentsData(minecraftData, fileList, minecraftPaths, launchData.PlayerName);
            var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);
            
            if (TryGetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo))
                await RestoreMissedItems(minecraftMissedInfo, cancellationToken);

            LaunchMinecraftProgress?.Invoke(LaunchProgress.StartGame, 0f);
            await Task.Delay(10, cancellationToken);
            
            Debug.WriteLine(launchArguments.Replace(" ", Environment.NewLine));
            Debug.WriteLine("Start game");
            
            await FileManager.StartProcess("java", launchArguments, exitedAction);
        }
        catch (TaskCanceledException e)
        {
            Console.WriteLine(e);
            LaunchMinecraftProgress?.Invoke(LaunchProgress.GameAborted, 0f);
        }
    }

    private async Task RestoreMissedItems(MinecraftMissedInfo missedInfo, CancellationToken cancellationToken)
    {
        for (var i = 0; i < missedInfo.DirectoriesToCreate.Count; i++)
            FileManager.CreateDirectory(missedInfo.DirectoriesToCreate[i]);

        for (var i = 0; i < missedInfo.CorruptedFiles.Count; i++)
            FileManager.Delete(missedInfo.CorruptedFiles[i]);

        if (missedInfo.DownloadQueue.Count > 0)
            await DownloadMissingFiles(missedInfo.DownloadQueue, missedInfo.TotalDownloadSize, cancellationToken);

        for (var i = 0; i < missedInfo.UnpackItems.Count; i++)
        {
            var (fileName, destination) = missedInfo.UnpackItems[i];
            FileManager.ExtractToDirectory(fileName, destination);
            
            Debug.WriteLine($"Unzip jar: {fileName}");
        }

        for (var i = 0; i < missedInfo.PathsToDelete.Count; i++)
            FileManager.Delete(missedInfo.PathsToDelete[i]);
    }

    private async Task DownloadMissingFiles(IEnumerable<(Uri source, string fileName)> downloadQueue, long totalSize,
        CancellationToken cancellationToken)
    {
        LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, 0f);
        await DownloadManager.DownloadFilesParallel(downloadQueue, cancellationToken, Callback);
        
        void Callback(long bytesReceived)
        {
            LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, (float)bytesReceived / totalSize);
        }
    }

    private static bool TryGetMissingInfo(MinecraftFileList minecraftFileList, MinecraftPaths minecraftPaths,
        out MinecraftMissedInfo minecraftMissedInfo)
    {
        minecraftMissedInfo = new MinecraftMissedInfo();
        
        CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Client);
        CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Server);
        
        if (minecraftFileList.Logging != null)
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Logging);

        for (var i = 0; i < minecraftFileList.LibraryFiles.Count; i++)
        {
            var libraryFile = minecraftFileList.LibraryFiles[i];
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, libraryFile);

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
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.AssetFiles[i]);

        return minecraftMissedInfo.DownloadQueue.Count > 0 ||
               minecraftMissedInfo.DirectoriesToCreate.Count > 0 ||
               minecraftMissedInfo.UnpackItems.Count > 0 ||
               minecraftMissedInfo.PathsToDelete.Count > 0;
    }

    private static void CheckFileAndDirectoryMissed(ref MinecraftMissedInfo missedInfo, IMinecraftFile minecraftFile)
    {
        if (!FileManager.FileExist(minecraftFile.FileName))
        {
            Debug.WriteLine($"File not exist: {minecraftFile.FileName}");
            missedInfo.DownloadQueue.Add((new Uri(minecraftFile.Url), minecraftFile.FileName));
            missedInfo.TotalDownloadSize += minecraftFile.Size;
        }
        else
        {
            var sha1 = FileManager.ComputeSha1(minecraftFile.FileName);
            if (sha1 != minecraftFile.Sha1)
            {
                Debug.WriteLine($"File {minecraftFile.FileName} corrupted ({sha1} != {minecraftFile.Sha1})");
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
    }

    private static List<PublicData.Version> GetVersionList(IEnumerable<MinecraftVersion> minecraftVersions)
    {
        return minecraftVersions.Select(version =>
            new PublicData.Version(version.Id, version.Url, MinecraftTypeConverter.GetVersionType(version.Type))).ToList();
    }
}