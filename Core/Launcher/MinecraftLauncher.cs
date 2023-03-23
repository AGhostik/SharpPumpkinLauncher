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
    private readonly FileManager _fileManager;

    private readonly Dictionary<string, MinecraftVersion> _minecraftVersions = new();

    public delegate void ProgressDelegate(string status, float progress01);
    public event ProgressDelegate? LaunchMinecraftProgress;

    public MinecraftLauncher()
    {
        _jsonManager = new JsonManager();
        _fileManager = new FileManager();
    }
    
    public async Task<Versions> GetAvailableVersions()
    {
        var versionsJson = await _fileManager.DownloadJsonAsync(VersionsUrl);
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
        
        LaunchMinecraftProgress?.Invoke("Get version data", 0f);

        var minecraftVersionJson = await _fileManager.DownloadJsonAsync(minecraftVersion.Url);
        var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

        if (minecraftData == null)
            return;

        var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, minecraftVersion.Id);

        LaunchMinecraftProgress?.Invoke("Get file data", 0f);
        
        var fileList = await GetFileList(minecraftData, minecraftPaths, minecraftVersion.Id);
        if (fileList == null)
            return;
        
        LaunchMinecraftProgress?.Invoke("Prepare to launch", 0f);

        var launchArgumentsData = new LaunchArgumentsData(_fileManager, minecraftData, fileList, minecraftPaths,
            launchData.PlayerName);

        var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);
        
        if (TryGetMissingInfo(fileList, minecraftPaths, out var minecraftMissedInfo))
            await RestoreMissedItems(minecraftMissedInfo);

        LaunchMinecraftProgress?.Invoke("Start game", 0f);
        _fileManager.StartProcess(launchData.JavaFile, launchArguments, exitedAction);
    }

    private async Task RestoreMissedItems(MinecraftMissedInfo missedInfo)
    {
        for (var i = 0; i < missedInfo.DirectoriesToCreate.Count; i++)
        {
            _fileManager.CreateDirectory(missedInfo.DirectoriesToCreate[i]);
        }

        var downloadingCount = missedInfo.DownloadQueue.Count;
        LaunchMinecraftProgress?.Invoke($"Download files (0/{downloadingCount})", 0f);
        
        await _fileManager.DownloadFiles(missedInfo.DownloadQueue, 
            index =>
            {
                var number = index + 1;
                LaunchMinecraftProgress?.Invoke($"Download files ({number}/{downloadingCount})",
                    number / (float)downloadingCount);
            });

        for (var i = 0; i < missedInfo.UnpackItems.Count; i++)
        {
            var (fileName, destination) = missedInfo.UnpackItems[i];
            _fileManager.ExtractToDirectory(fileName, destination);
        }
    }

    private bool TryGetMissingInfo(MinecraftFileList minecraftFileList, MinecraftPaths minecraftPaths,
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
                var unpackDirectory = minecraftPaths.NativesDirectory;
                if (!_fileManager.DirectoryExist(unpackDirectory) && !minecraftMissedInfo.DirectoriesToCreate.Contains(unpackDirectory))
                    minecraftMissedInfo.DirectoriesToCreate.Add(unpackDirectory);

                minecraftMissedInfo.UnpackItems.Add((libraryFile.FileName, unpackDirectory));
                //todo: add file removing
            }
        }
        
        for (var i = 0; i < minecraftFileList.AssetFiles.Count; i++)
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.AssetFiles[i]);

        return minecraftMissedInfo.DownloadQueue.Count > 0 ||
               minecraftMissedInfo.DirectoriesToCreate.Count > 0 ||
               minecraftMissedInfo.UnpackItems.Count > 0;
    }

    private void CheckFileAndDirectoryMissed(ref MinecraftMissedInfo missedInfo, IMinecraftFile minecraftFile)
    {
        if (CheckFileExist(minecraftFile, out var downloadInfo))
            missedInfo.DownloadQueue.Add(downloadInfo);
        
        if (CheckFileDirectoryExist(minecraftFile, out var directory) && !missedInfo.DirectoriesToCreate.Contains(directory))
            missedInfo.DirectoriesToCreate.Add(directory);
    }

    private bool CheckFileExist(IMinecraftFile minecraftFile, out (Uri, string) downloadInfo)
    {
        var fileName = minecraftFile.FileName;
        var url = minecraftFile.Url;

        if (_fileManager.FileExist(fileName))
        {
            downloadInfo = default;
            return false;
        }
        
        downloadInfo = (new Uri(url), fileName);
        return true;
    }
    
    private bool CheckFileDirectoryExist(IMinecraftFile minecraftFile, out string directory)
    {
        var fileName = minecraftFile.FileName;

        if (!_fileManager.DirectoryExist(fileName))
        {
            directory = _fileManager.GetPathDirectory(fileName) ?? string.Empty;
            return true;
        }

        directory = string.Empty;
        return false;
    }

    private async Task<MinecraftFileList?> GetFileList(MinecraftData data, MinecraftPaths minecraftPaths,
        string minecraftVersionId)
    {
        var assetsJson = await _fileManager.DownloadJsonAsync(data.AssetsUrl);
        var assets = _jsonManager.GetAssets(assetsJson);
        if (assets == null)
            return null;
        
        var client = new MinecraftFile(data.Client.Url, $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}.jar");
        var server = new MinecraftFile(data.Server.Url,
            $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}-server.jar");
        
        var librariesFiles = GetLibrariesFiles(data.Libraries, minecraftPaths);
        var assetsFiles = GetAssetsFiles(assets, minecraftPaths);
        
        var minecraftFileList = new MinecraftFileList(client, server, librariesFiles, assetsFiles);

        if (data.LoggingData != null)
        {
            minecraftFileList.Logging = new MinecraftFile(data.LoggingData.File.Url,
                $"{minecraftPaths.VersionDirectory}\\log4j2.xml");
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
            
            //todo:
            // if (isNatives)
            // {
            //     var fileName = $"{minecraftPaths.TemporaryDirectory}\\{path}";
            //     var minecraftLibraryFile = new MinecraftLibraryFile(url, fileName)
            //     {
            //         NeedUnpack = true
            //     };
            //
            //     minecraftLibraryFile.Delete.Add(fileName);
            //     minecraftFileList.LibraryFiles.Add(minecraftLibraryFile);
            // }
            // else
            // {
                var fileName = $"{minecraftPaths.LibrariesDirectory}\\{libraryData.File.Path}";
                var minecraftLibraryFile = new MinecraftLibraryFile(libraryData.File.Url, fileName);
                result.Add(minecraftLibraryFile);
            // }
        }

        return result;
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
            var minecraftFile = new MinecraftFile($"{AssetsUrl}/{subDirectory}/{hashString}", fileName);
            
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