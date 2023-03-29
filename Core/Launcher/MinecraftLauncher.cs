using System.Diagnostics;
using JsonReader;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Manifest;
using Launcher.Data;
using Launcher.PublicData;
using Launcher.Tools;
using Versions = Launcher.PublicData.Versions;

namespace Launcher;

public class MinecraftLauncher
{
    private const string AssetsUrl = "https://resources.download.minecraft.net";
    private const string VersionsUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
    
    private readonly JsonManager _jsonManager;

    private readonly Dictionary<string, MinecraftVersion> _minecraftVersions = new();

    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;

    public MinecraftLauncher()
    {
        _jsonManager = new JsonManager();
    }
    
    public async Task<Versions> GetAvailableVersions()
    {
        var versionsJson = await FileManager.DownloadJsonAsync(VersionsUrl);
        var versions = _jsonManager.GetVersions(versionsJson);

        _minecraftVersions.Clear();
        
        if (versions == null)
            return Versions.Empty;
        
        AddMinecraftVersionToDictionary(versions.Alpha);
        AddMinecraftVersionToDictionary(versions.Beta);
        AddMinecraftVersionToDictionary(versions.Snapshot);
        AddMinecraftVersionToDictionary(versions.Release);

        return new Versions(
            versions.Latest,
            versions.LatestSnapshot,
            GetVersionList(versions.Release), 
            GetVersionList(versions.Snapshot), 
            GetVersionList(versions.Beta), 
            GetVersionList(versions.Alpha));
    }
    
    public async Task LaunchMinecraft(LaunchData launchData, Action? exitedAction = null)
    {
        if (string.IsNullOrEmpty(launchData.VersionId))
            return;
        
        if (!_minecraftVersions.TryGetValue(launchData.VersionId, out var minecraftVersion))
            return;
        
        LaunchMinecraftProgress?.Invoke(LaunchProgress.GetVersionData, 0f);

        var minecraftVersionJson = await FileManager.DownloadJsonAsync(minecraftVersion.Url);
        var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

        if (minecraftData == null)
            return;

        var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, minecraftVersion.Id);

        LaunchMinecraftProgress?.Invoke(LaunchProgress.GetFileList, 0f);
        
        var fileList = await GetFileList(minecraftData, minecraftPaths, minecraftVersion.Id);
        if (fileList == null)
            return;
        
        LaunchMinecraftProgress?.Invoke(LaunchProgress.GetLaunchArguments, 0f);

        var launchArgumentsData =
            new LaunchArgumentsData(minecraftData, fileList, minecraftPaths, launchData.PlayerName);
        var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);
        
        if (TryGetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo))
            await RestoreMissedItems(minecraftMissedInfo);

        LaunchMinecraftProgress?.Invoke(LaunchProgress.StartGame, 0f);
        await Task.Delay(10);
        
        Debug.WriteLine(launchArguments.Replace(" ", Environment.NewLine));
        
        await FileManager.StartProcess("java", launchArguments, exitedAction);
    }

    private async Task RestoreMissedItems(MinecraftMissedInfo missedInfo)
    {
        for (var i = 0; i < missedInfo.DirectoriesToCreate.Count; i++)
            FileManager.CreateDirectory(missedInfo.DirectoriesToCreate[i]);

        for (var i = 0; i < missedInfo.CorruptedFiles.Count; i++)
            FileManager.Delete(missedInfo.CorruptedFiles[i]);

        if (missedInfo.DownloadQueue.Count > 0)
            await DownloadMissingFiles(missedInfo.DownloadQueue, missedInfo.TotalDownloadSize);

        for (var i = 0; i < missedInfo.UnpackItems.Count; i++)
        {
            var (fileName, destination) = missedInfo.UnpackItems[i];
            FileManager.ExtractToDirectory(fileName, destination);
        }

        for (var i = 0; i < missedInfo.PathsToDelete.Count; i++)
            FileManager.Delete(missedInfo.PathsToDelete[i]);
    }

    private async Task DownloadMissingFiles(IEnumerable<(Uri source, string fileName)> downloadQueue, long totalSize)
    {
        LaunchMinecraftProgress?.Invoke(LaunchProgress.DownloadFiles, 0f);
        await FileManager.DownloadFilesParallel(downloadQueue, Callback);
        
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
        CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.AssetsIndex);
        
        if (minecraftFileList.Logging != null)
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Logging);

        var anyLibraryNeedUnpack = false;
        for (var i = 0; i < minecraftFileList.LibraryFiles.Count; i++)
        {
            var libraryFile = minecraftFileList.LibraryFiles[i];
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, libraryFile);

            if (libraryFile.NeedUnpack)
            {
                var unpackDirectory = minecraftPaths.NativesDirectory;
                if (!FileManager.DirectoryExist(unpackDirectory) && !minecraftMissedInfo.DirectoriesToCreate.Contains(unpackDirectory))
                    minecraftMissedInfo.DirectoriesToCreate.Add(unpackDirectory);

                minecraftMissedInfo.UnpackItems.Add((libraryFile.FileName, unpackDirectory));
                minecraftMissedInfo.PathsToDelete.Add(libraryFile.FileName);
                anyLibraryNeedUnpack = true;
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

        if (anyLibraryNeedUnpack)
        {
            if (!minecraftMissedInfo.PathsToDelete.Contains(minecraftPaths.TemporaryDirectory))
                minecraftMissedInfo.PathsToDelete.Add(minecraftPaths.TemporaryDirectory);
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
            missedInfo.DownloadQueue.Add((new Uri(minecraftFile.Url), minecraftFile.FileName));
            missedInfo.TotalDownloadSize += minecraftFile.Size;
        }
        else
        {
            var sha1 = FileManager.ComputeSha1(minecraftFile.FileName);
            if (sha1 != minecraftFile.Sha1)
            {
                Debug.WriteLine($"File {minecraftFile.FileName} corrupted");
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

    private async Task<MinecraftFileList?> GetFileList(MinecraftData data, MinecraftPaths minecraftPaths,
        string minecraftVersionId)
    {
        var assetsJson = await FileManager.DownloadJsonAsync(data.AssetsIndex.Url);
        var assets = _jsonManager.GetAssets(assetsJson);
        if (assets == null)
            return null;
        
        var client = new MinecraftFile(data.Client.Url, data.Client.Size, data.Client.Sha1,
            $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}.jar");
        
        var server = new MinecraftFile(data.Server.Url, data.Server.Size, data.Server.Sha1,
            $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}-server.jar");
        
        var assetsIndex = new MinecraftFile(data.AssetsIndex.Url, data.AssetsIndex.Size, data.AssetsIndex.Sha1,
            $"{minecraftPaths.AssetsDirectory}\\indexes\\{FileManager.GetFileName(data.AssetsIndex.Url)}");
        
        var librariesFiles = GetLibrariesFiles(data.Libraries, minecraftPaths);
        var assetsFiles = GetAssetsFiles(assets, minecraftPaths);
        
        var minecraftFileList = new MinecraftFileList(client, server, assetsIndex, librariesFiles, assetsFiles);

        if (data.LoggingData != null)
        {
            minecraftFileList.Logging = new MinecraftFile(data.LoggingData.File.Url, data.LoggingData.File.Size,
                data.LoggingData.File.Sha1, $"{minecraftPaths.VersionDirectory}\\log4j2.xml");
        }

        return minecraftFileList;
    }

    private static IReadOnlyList<MinecraftLibraryFile> GetLibrariesFiles(IReadOnlyList<Library> libraries,
        MinecraftPaths minecraftPaths)
    {
        var result = new List<MinecraftLibraryFile>(libraries.Count);
        for (var i = 0; i < libraries.Count; i++)
        {
            var libraryData = libraries[i];
            
            if (!OsRuleManager.IsAllowed(libraryData.Rules))
                continue;

            if (OperatingSystem.IsWindows())
            {
                if (!string.IsNullOrEmpty(libraryData.NativesWindows) && libraryData.NativesWindowsFile != null)
                {
                    result.Add(GetNativeLibraryFile(libraryData.NativesWindowsFile, minecraftPaths.TemporaryDirectory,
                        libraryData.Delete));
                }
            }
            else if (OperatingSystem.IsLinux())
            {
                if (!string.IsNullOrEmpty(libraryData.NativesLinux) && libraryData.NativesLinuxFile != null)
                {
                    result.Add(GetNativeLibraryFile(libraryData.NativesLinuxFile, minecraftPaths.TemporaryDirectory,
                        libraryData.Delete));
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                if (!string.IsNullOrEmpty(libraryData.NativesOsx) && libraryData.NativesOsxFile != null)
                {
                    result.Add(GetNativeLibraryFile(libraryData.NativesOsxFile, minecraftPaths.TemporaryDirectory,
                        libraryData.Delete));
                }
            }

            if (libraryData.File != null)
            {
                var fileName = $"{minecraftPaths.LibrariesDirectory}\\{libraryData.File.Path}";
                var minecraftLibraryFile = new MinecraftLibraryFile(libraryData.File.Url, libraryData.File.Size,
                    libraryData.File.Sha1, fileName);
                result.Add(minecraftLibraryFile);
            }
        }

        return result;
    }

    private static MinecraftLibraryFile GetNativeLibraryFile(LibraryFile file, string temporaryDirectory,
        IReadOnlyList<string> deleteFiles)
    {
        var nativeFileName = $"{temporaryDirectory}\\{file.Path}";
        var minecraftNativeLibraryFile = new MinecraftLibraryFile(file.Url, file.Size, file.Sha1, nativeFileName)
        {
            NeedUnpack = true,
            Delete = deleteFiles
        };
        return minecraftNativeLibraryFile;
    }
    
    private static IReadOnlyList<MinecraftFile> GetAssetsFiles(IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths)
    {
        var result = new List<MinecraftFile>(assets.Count);
        for (var i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];

            var hashString = asset.Hash;
            
            if (hashString.Length < 2)
                continue;
            
            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var fileName = $"{minecraftPaths.AssetsDirectory}\\objects\\{subDirectory}\\{hashString}";
            var minecraftFile = new MinecraftFile($"{AssetsUrl}/{subDirectory}/{hashString}", asset.Size, asset.Hash,
                fileName);
            
            result.Add(minecraftFile);
        }
        
        return result;
    }
    
    private static List<PublicData.Version> GetVersionList(IEnumerable<MinecraftVersion> minecraftVersions)
    {
        return minecraftVersions
            .Select(version => new PublicData.Version(version.Id, GetVersionType(version.Type))).ToList();
    }

    private static VersionType GetVersionType(MinecraftType version)
    {
        return version switch
        {
            MinecraftType.Release => VersionType.Release,
            MinecraftType.Snapshot => VersionType.Snapshot,
            MinecraftType.Beta => VersionType.Beta,
            MinecraftType.Alpha => VersionType.Alpha,
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, null)
        };
    }

    private void AddMinecraftVersionToDictionary(IReadOnlyList<MinecraftVersion> minecraftVersions)
    {
        for (var i = 0; i < minecraftVersions.Count; i++)
            _minecraftVersions.Add(minecraftVersions[i].Id, minecraftVersions[i]);
    }
}