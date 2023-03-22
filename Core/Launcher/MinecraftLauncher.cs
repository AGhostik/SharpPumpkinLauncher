using JsonReader;
using JsonReader.Assets;
using JsonReader.Game;
using JsonReader.PublicData;
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

        List<PublicData.Version> GetVersionList(IEnumerable<MinecraftVersion> minecraftVersions)
        {
            return minecraftVersions
                .Select(version => new PublicData.Version(version.Id, GetVersionType(version.Type))).ToList();
        }

        VersionType GetVersionType(MinecraftType version)
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

        void AddMinecraftVersionToDictionary(IReadOnlyList<MinecraftVersion> minecraftVersions)
        {
            for (var i = 0; i < minecraftVersions.Count; i++)
                _minecraftVersions.Add(minecraftVersions[i].Id, minecraftVersions[i]);
        }
    }
    
    public async Task LaunchMinecraft(LaunchData launchData, Action? exitedAction = null)
    {
        if (string.IsNullOrEmpty(launchData.VersionId))
            return;
        
        if (!_minecraftVersions.TryGetValue(launchData.VersionId, out var minecraftVersion))
            return;
        
        LaunchMinecraftProgress?.Invoke("Get version data", 0f);

        var minecraftVersionJson = await _fileManager.DownloadJsonAsync(minecraftVersion.Url);
        var minecraftVersionData = _jsonManager.GetVersionData(minecraftVersionJson);

        if (minecraftVersionData == null)
            return;

        var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, minecraftVersion.Id);

        LaunchMinecraftProgress?.Invoke("Get file data", 0f);
        
        var fileList = await GetFileList(minecraftVersionData, minecraftPaths, minecraftVersion.Id);
        if (fileList == null)
            return;
        
        if (fileList.Client == null || string.IsNullOrEmpty(fileList.Client.FileName))
            return;
        
        LaunchMinecraftProgress?.Invoke("Prepare to launch", 0f);

        var launchArgumentsData = new LaunchArgumentsData(_fileManager, minecraftVersionData, fileList, minecraftPaths,
            launchData.PlayerName);

        var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftVersionData, launchArgumentsData);
        
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
        
        if (minecraftFileList.Client != null)
            CheckFileAndDirectoryMissed(ref minecraftMissedInfo, minecraftFileList.Client);
        
        if (minecraftFileList.Server != null)
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

    private async Task<MinecraftFileList?> GetFileList(MinecraftVersionData data, MinecraftPaths minecraftPaths,
        string minecraftVersionId)
    {
        if (!TryGetMinecraftInfo(data, out var clientUrl, out var serverUrl, out var loggingUrl, out var assetsUrl,
                out var libraries))
        {
            return null;
        }

        var assetsJson = await _fileManager.DownloadJsonAsync(assetsUrl);
        var assetData = _jsonManager.GetAssetsData(assetsJson);

        if (assetData?.AssetList == null)
            return null;

        var minecraftFileList = new MinecraftFileList
        {
            Client = new MinecraftFile(clientUrl, $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}.jar"),
            Server = new MinecraftFile(serverUrl, $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}-server.jar"),
            Logging = new MinecraftFile(loggingUrl, $"{minecraftPaths.VersionDirectory}\\log4j2.xml")
        };

        AddLibraries(ref minecraftFileList, libraries, minecraftPaths);
        AddAssets(ref minecraftFileList, assetData.AssetList.Values.ToList(), minecraftPaths);

        return minecraftFileList;
    }
    
    private static bool TryGetMinecraftInfo(MinecraftVersionData data, out string clientUrl, out string serverUrl, 
        out string loggingUrl, out string assetsUrl, out IReadOnlyList<LibraryData> libraries)
    {
        if (data.Downloads?.Client?.Url == null || data.Downloads.Server?.Url == null ||
            data.Logging?.Client?.File?.Url == null || data.AssetIndex?.Url == null || data.Library == null)
        {
            clientUrl = string.Empty;
            serverUrl = string.Empty;
            loggingUrl = string.Empty;
            assetsUrl = string.Empty;
            libraries = Array.Empty<LibraryData>();
            return false;
        }
        
        clientUrl = data.Downloads.Client.Url;
        serverUrl = data.Downloads.Server.Url;
        loggingUrl = data.Logging.Client.File.Url;
        assetsUrl = data.AssetIndex.Url;
        libraries = data.Library;
        return true;
    }

    private static void AddLibraries(ref MinecraftFileList minecraftFileList, IReadOnlyList<LibraryData> libraries,
        MinecraftPaths minecraftPaths)
    {
        for (var i = 0; i < libraries.Count; i++)
        {
            var libraryData = libraries[i];
            
            if (!OsRuleManager.IsAllowed(libraryData.Rules))
                continue;
            
            GetLibraryInfo(libraryData, out var path, out var url, out var isNatives);
            
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(url))
                continue;
            
            if (isNatives)
            {
                var fileName = $"{minecraftPaths.TemporaryDirectory}\\{path}";
                var minecraftLibraryFile = new MinecraftLibraryFile(url, fileName)
                {
                    NeedUnpack = true
                };

                minecraftLibraryFile.Delete.Add(fileName);
                minecraftFileList.LibraryFiles.Add(minecraftLibraryFile);
            }
            else
            {
                var fileName = $"{minecraftPaths.LibrariesDirectory}\\{path}";
                var minecraftLibraryFile = new MinecraftLibraryFile(url, fileName);
                minecraftFileList.LibraryFiles.Add(minecraftLibraryFile);
            }
        }
    }
    
    private static void GetLibraryInfo(LibraryData library, out string? path, out string? url, out bool isNatives)
    {
        path = library.Downloads?.Artifact?.Path;
        url = library.Downloads?.Artifact?.Url;
        isNatives = library.Natives != null;
    }
    
    private static void AddAssets(ref MinecraftFileList minecraftFileList, IReadOnlyList<AssetData> assets,
        MinecraftPaths minecraftPaths)
    {
        for (var i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];

            var hashString = asset.Hash;
            
            if (string.IsNullOrEmpty(hashString) || hashString.Length < 2)
                continue;
            
            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var fileName = $"{minecraftPaths.AssetsDirectory}\\objects\\{subDirectory}\\{hashString}";
            var minecraftFile = new MinecraftFile($"{AssetsUrl}/{subDirectory}/{hashString}", fileName);
            
            minecraftFileList.AssetFiles.Add(minecraftFile);
        }
    }
}