using JsonReader;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Forge;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using Launcher.Data;
using Launcher.Interfaces;
using Launcher.PublicData;
using SimpleLogger;
using ForgeVersion = Launcher.PublicData.ForgeVersion;

namespace Launcher.Tools;

internal sealed class InstallerData : IForgeInstallerData
{
    private readonly JsonManager _jsonManager;

    public InstallerData(JsonManager jsonManager)
    {
        _jsonManager = jsonManager;
    }
    
    public async Task<(MinecraftData?, ErrorCode)> GetAndSaveMinecraftData(string? url, string versionId,
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
        
        var directoryCreated = FileManager.CreateDirectory(minecraftPaths.VersionDirectory);
        if (!directoryCreated)
            return (null, ErrorCode.CreateDirectory);
            
        var versionJsonCreated =
            await FileManager.WriteFile($"{minecraftPaths.VersionDirectory}\\{versionId}.json",
                minecraftVersionJson);
        
        if (!versionJsonCreated)
            return (null, ErrorCode.CreateFile);

        return (minecraftData, ErrorCode.NoError);
    }

    public async Task<(IReadOnlyList<Asset>?, ErrorCode)> GetAndSaveAssetsData(MinecraftData minecraftData,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var assetsJson = await DownloadManager.DownloadJsonAsync(minecraftData.AssetsIndex.Url, cancellationToken);
            
        if (string.IsNullOrEmpty(assetsJson))
            return (null, ErrorCode.Download);
            
        var directoryCreated = FileManager.CreateDirectory(minecraftPaths.AssetsIndexesDirectory);
        if (!directoryCreated)
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

    public async Task<(RuntimeFiles?, ErrorCode)> GetAndSaveRuntimes(MinecraftData minecraftData,
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
        
        var directoryCreated = FileManager.CreateDirectory(minecraftPaths.RuntimeDirectory);
        if (!directoryCreated)
            return (null, ErrorCode.CreateDirectory);
        
        var runtimeFilesJsonCreated = await FileManager.WriteFile(
            $"{minecraftPaths.RuntimeDirectory}\\{minecraftData.JavaVersion.Component}-{OsRuleManager.CurrentOsName}.json",
            currentRuntimeJson);
        
        if (!runtimeFilesJsonCreated)
            return (null, ErrorCode.CreateFile);

        return (runtimeFiles, ErrorCode.NoError);
    }
    
    public async Task<(ForgeInfo?, ErrorCode)> GetAndSaveForge(ForgeVersion forgeVersion,
        MinecraftPaths minecraftPaths, CancellationToken cancellationToken)
    {
        var forgeJson = await DownloadManager.DownloadJsonAsync(forgeVersion.Url, cancellationToken);
        
        if (string.IsNullOrEmpty(forgeJson))
            return (null, ErrorCode.Download);

        var forgeInfo = _jsonManager.GetForgeInfo(forgeJson);

        if (forgeInfo == null)
            return (null, ErrorCode.ForgeData);
        
        var directoryCreated = FileManager.CreateDirectory(minecraftPaths.VersionDirectory);
        if (!directoryCreated)
            return (null, ErrorCode.CreateDirectory);
        
        var forgeInfoJsonCreated = await FileManager.WriteFile(
            $"{minecraftPaths.VersionDirectory}\\FORGE-{forgeVersion.Id}.json", forgeJson);
        
        if (!forgeInfoJsonCreated)
            return (null, ErrorCode.CreateFile);
        
        return (forgeInfo, ErrorCode.NoError);
    }
    
    public async Task<(MinecraftData?, ErrorCode)> ReadMinecraftData(MinecraftPaths minecraftPaths, string versionId,
        CancellationToken cancellationToken)
    {
        var minecraftVersionJson =
            await FileManager.ReadFile($"{minecraftPaths.VersionDirectory}\\{versionId}.json",
                cancellationToken);

        if (string.IsNullOrEmpty(minecraftVersionJson))
            return (null, ErrorCode.ReadFile);

        var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

        if (minecraftData == null)
            return (null, ErrorCode.MinecraftData);

        return (minecraftData, ErrorCode.NoError);
    }
    
    public async Task<IReadOnlyList<Asset>?> ReadAssetsData(MinecraftData minecraftData, MinecraftPaths minecraftPaths, 
        CancellationToken cancellationToken)
    {
        var assetsDataJson = await FileManager.ReadFile(
            $"{minecraftPaths.AssetsIndexesDirectory}\\{minecraftData.AssetsVersion}.json",
            cancellationToken);

        if (string.IsNullOrEmpty(assetsDataJson))
            return null;
            
        var assetsData = _jsonManager.GetAssets(assetsDataJson);

        return assetsData;
    }
    
    public async Task<RuntimeFiles?> ReadRuntimesData(MinecraftData minecraftData, MinecraftPaths minecraftPaths,
        CancellationToken cancellationToken)
    {
        var runtimesDataJson = await FileManager.ReadFile(
            $"{minecraftPaths.RuntimeDirectory}\\{minecraftData.JavaVersion.Component}-{OsRuleManager.CurrentOsName}.json",
            cancellationToken);

        if (string.IsNullOrEmpty(runtimesDataJson))
            return null;
            
        var runtimeFiles = _jsonManager.GetRuntimeFiles(runtimesDataJson);

        return runtimeFiles;
    }

    public async Task<(ForgeInfo?, ErrorCode)> ReadForgeInfo(string forgeVersionId, MinecraftPaths minecraftPaths, 
        CancellationToken cancellationToken)
    {
        var forgeInfoJson =
            await FileManager.ReadFile($"{minecraftPaths.VersionDirectory}\\FORGE-{forgeVersionId}.json",
                cancellationToken);

        if (string.IsNullOrEmpty(forgeInfoJson))
            return (null, ErrorCode.ReadFile);
            
        var forgeInfo = _jsonManager.GetForgeInfo(forgeInfoJson);

        if (forgeInfo == null)
            return (null, ErrorCode.ForgeData);

        return (forgeInfo, ErrorCode.NoError);
    }
    
    public MinecraftFileList GetFileList(string versionId, MinecraftData data, RuntimeFiles runtimeFiles,
        IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths)
    {
        return GetFileListInternal(versionId, data, runtimeFiles, assets, minecraftPaths);
    }
    
    public MinecraftFileList GetForgeFileList(string versionId, MinecraftData data, ForgeInfo forgeInfo, 
        RuntimeFiles runtimeFiles, IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths)
    {
        return GetFileListInternal(versionId, data, runtimeFiles, assets, minecraftPaths, forgeInfo);
    }
    
    private static MinecraftFileList GetFileListInternal(string versionId, MinecraftData data, RuntimeFiles runtimeFiles,
        IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths, ForgeInfo? forgeInfo = null)
    {
        var client = new MinecraftFile(data.Client.Url, data.Client.Size, data.Client.Sha1,
            $"{minecraftPaths.VersionDirectory}\\{versionId}.jar");

        MinecraftFile? server = null;
        if (data.Server != null)
        {
            server = new MinecraftFile(data.Server.Url, data.Server.Size, data.Server.Sha1,
                $"{minecraftPaths.VersionDirectory}\\{versionId}-server.jar");
        }

        var librariesFiles = GetLibrariesFiles(data, forgeInfo, minecraftPaths);

        var assetsFiles = data.IsLegacyAssets()
            ? GetLegacyAssetsFiles(assets, minecraftPaths)
            : GetAssetsFiles(assets, minecraftPaths);

        var runtimeType = data.JavaVersion.Component;
        var javaRuntimeFiles = GetRuntimeFiles(runtimeFiles, runtimeType, minecraftPaths);
        
        var minecraftFileList = new MinecraftFileList(client, server, librariesFiles, assetsFiles, javaRuntimeFiles);

        if (data.LoggingData != null)
        {
            minecraftFileList.Logging = new MinecraftFile(data.LoggingData.File.Url, data.LoggingData.File.Size,
                data.LoggingData.File.Sha1, $"{minecraftPaths.VersionDirectory}\\log4j2.xml");
        }

        return minecraftFileList;
    }
    
    private static IReadOnlyList<MinecraftLibraryFile> GetLibrariesFiles(MinecraftData data, ForgeInfo? forgeInfo,
        MinecraftPaths minecraftPaths)
    {
        IReadOnlyList<MinecraftLibraryFile> librariesFiles;
        if (forgeInfo != null)
        {
            var forgeAndMinecraftLibs = new List<Library>(forgeInfo.Libraries);
            forgeAndMinecraftLibs.AddRange(forgeInfo.ForgeInstall.Libraries);
            forgeAndMinecraftLibs.AddRange(data.Libraries);
            librariesFiles = GetLibrariesFilesInternal(forgeAndMinecraftLibs, minecraftPaths);
        }
        else
        {
            librariesFiles = GetLibrariesFilesInternal(data.Libraries, minecraftPaths);
        }

        return librariesFiles;
    }
    
    private static IReadOnlyList<MinecraftLibraryFile> GetLibrariesFilesInternal(IReadOnlyList<Library> libraries,
        MinecraftPaths minecraftPaths)
    {
        var result = new List<MinecraftLibraryFile>(libraries.Count);
        for (var i = 0; i < libraries.Count; i++)
        {
            var libraryData = libraries[i];
            
            if (!OsRuleManager.IsAllowed(libraryData.Rules))
                continue;

            if (!libraryData.IsNative)
            {
                if (libraryData.File != null)
                {
                    var fileName = $"{minecraftPaths.LibrariesDirectory}\\{libraryData.File.Path}";
                    var minecraftLibraryFile = new MinecraftLibraryFile(libraryData.File.Url, libraryData.File.Size,
                        libraryData.File.Sha1, fileName);
                    result.Add(minecraftLibraryFile);
                }
            }
            else
            {
                if (OperatingSystem.IsWindows())
                {
                    if (!string.IsNullOrEmpty(libraryData.NativesWindows) && libraryData.NativesWindowsFile != null)
                    {
                        result.Add(GetNativeLibraryFile(libraryData.NativesWindowsFile,
                            minecraftPaths.LibrariesDirectory,
                            libraryData.Delete));
                    }
                }
                else if (OperatingSystem.IsLinux())
                {
                    if (!string.IsNullOrEmpty(libraryData.NativesLinux) && libraryData.NativesLinuxFile != null)
                    {
                        result.Add(GetNativeLibraryFile(libraryData.NativesLinuxFile, minecraftPaths.LibrariesDirectory,
                            libraryData.Delete));
                    }
                }
                else if (OperatingSystem.IsMacOS())
                {
                    if (!string.IsNullOrEmpty(libraryData.NativesOsx) && libraryData.NativesOsxFile != null)
                    {
                        result.Add(GetNativeLibraryFile(libraryData.NativesOsxFile, minecraftPaths.LibrariesDirectory,
                            libraryData.Delete));
                    }
                }
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
            {
                Logger.Log($"Invalid asset hash; name: '{asset.Name}', hash: '{asset.Hash}'");
                continue;
            }
            
            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var fileName = $"{minecraftPaths.AssetsObjectsDirectory}\\{subDirectory}\\{hashString}";
            var minecraftFile = new MinecraftFile($"{WellKnownUrls.AssetsUrl}/{subDirectory}/{hashString}", asset.Size,
                asset.Hash, fileName);
            
            result.Add(minecraftFile);
        }
        
        return result;
    }
    
    private static IReadOnlyList<MinecraftFile> GetLegacyAssetsFiles(IReadOnlyList<Asset> assets,
        MinecraftPaths minecraftPaths)
    {
        var result = new List<MinecraftFile>(assets.Count);
        for (var i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];
            var hashString = asset.Hash;

            if (hashString.Length < 2)
            {
                Logger.Log($"Invalid asset hash; name: '{asset.Name}', hash: '{asset.Hash}'");
                continue;
            }
            
            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var fileName = $"{minecraftPaths.AssetsLegacyDirectory}\\{asset.Name}";
            var minecraftFile = new MinecraftFile($"{WellKnownUrls.AssetsUrl}/{subDirectory}/{hashString}", asset.Size,
                asset.Hash, fileName);
            
            result.Add(minecraftFile);
        }
        
        return result;
    }
    
    private static IReadOnlyList<MinecraftFile> GetRuntimeFiles(RuntimeFiles runtimeFiles, string runtimeType,
        MinecraftPaths minecraftPaths)
    {
        var files = new List<MinecraftFile>();
        for (var i = 0; i < runtimeFiles.Files.Count; i++)
        {
            var file = runtimeFiles.Files[i];
            var fileName =
                $"{minecraftPaths.RuntimeDirectory}\\{runtimeType}\\{OsRuleManager.CurrentOsName}\\{file.Path}";
            files.Add(new MinecraftFile(file.Url, file.Size, file.Sha1, fileName));
        }

        return files;
    }
    
    public ErrorCode GetMissingInfo(MinecraftFileList minecraftFileList, MinecraftPaths minecraftPaths,
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
            if (string.IsNullOrEmpty(sha1) || string.IsNullOrEmpty(minecraftFile.Sha1))
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

    public MinecraftLaunchFiles GetLaunchFiles(string versionId, MinecraftData data, MinecraftPaths minecraftPaths)
    {
        return GetLaunchFilesInternal(versionId, data, minecraftPaths);
    }
    
    public MinecraftLaunchFiles GetForgeLaunchFiles(string versionId, MinecraftData data, ForgeInfo forgeInfo,
        MinecraftPaths minecraftPaths)
    {
        return GetLaunchFilesInternal(versionId, data, minecraftPaths, forgeInfo);
    }
    
    private static MinecraftLaunchFiles GetLaunchFilesInternal(string versionId, MinecraftData data, 
        MinecraftPaths minecraftPaths, ForgeInfo? forgeInfo = null)
    {
        var client = FileManager.GetFullPath($"{minecraftPaths.VersionDirectory}\\{versionId}.jar") ?? string.Empty;

        var libraries = new List<string>();
        var librariesFiles = GetLibrariesFilesForLaunch(data, forgeInfo, minecraftPaths);
        for (var i = 0; i < librariesFiles.Count; i++)
        {
            var libraryFile = librariesFiles[i];
            if (libraryFile.NeedUnpack)
                continue;

            var path = FileManager.GetFullPath(libraryFile.FileName);
            if (string.IsNullOrEmpty(path))
                continue;
            
            libraries.Add(path);
        }

        string logging;
        if (data.LoggingData != null)
            logging = FileManager.GetFullPath($"{minecraftPaths.VersionDirectory}\\log4j2.xml") ?? string.Empty;
        else
            logging = string.Empty;
        
        var javaFile = 
            $"{minecraftPaths.RuntimeDirectory}\\{data.JavaVersion.Component}\\{OsRuleManager.GetJavaExecutablePath()}";

        return new MinecraftLaunchFiles(client, logging, javaFile, libraries);
    }
    
    private static IReadOnlyList<MinecraftLibraryFile> GetLibrariesFilesForLaunch(MinecraftData data, 
        ForgeInfo? forgeInfo, MinecraftPaths minecraftPaths)
    {
        IReadOnlyList<MinecraftLibraryFile> librariesFiles;
        if (forgeInfo != null)
        {
            var forgeAndMinecraftLibs = new List<Library>(forgeInfo.Libraries);
            forgeAndMinecraftLibs.AddRange(data.Libraries);
            librariesFiles = GetLibrariesFilesInternal(forgeAndMinecraftLibs, minecraftPaths);
        }
        else
        {
            librariesFiles = GetLibrariesFilesInternal(data.Libraries, minecraftPaths);
        }

        return librariesFiles;
    }
}