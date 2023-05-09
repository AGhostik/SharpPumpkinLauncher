using System.Diagnostics.CodeAnalysis;
using JsonReader;
using JsonReader.PublicData.Manifest;
using Launcher.PublicData;
using Launcher.Tools;
using SimpleLogger;
using Version = Launcher.PublicData.Version;
using Versions = Launcher.PublicData.Versions;

namespace Launcher.Vanilla;

internal sealed class VanillaVersionsLoader
{
    private readonly JsonManager _jsonManager;
    
    private Versions? _onlineVersions;
    private Versions? _offlineVersions;

    public VanillaVersionsLoader(JsonManager jsonManager)
    {
        _jsonManager = jsonManager;
    }

    public bool TryGetVersion(string versionId, [NotNullWhen(true)]out Version? version)
    {
        if (_onlineVersions != null)
        {
            if (_onlineVersions.AllVersions.TryGetValue(versionId, out version))
                return true;
        }

        if (_offlineVersions != null)
        {
            if (_offlineVersions.AllVersions.TryGetValue(versionId, out version))
                return true;
        }

        version = null;
        return false;
    }

    public async Task<Versions> GetOnlineAvailableVersions(CancellationToken cancellationToken)
    {
        if (_onlineVersions != null)
            return _onlineVersions;
        
        var versionsJson = await DownloadManager.DownloadJsonAsync(WellKnownUrls.VersionsUrl, cancellationToken);
        var versions = _jsonManager.GetVersions(versionsJson);
        
        if (versions == null)
            return Versions.Empty;

        var result = new Versions(
            versions.Latest,
            versions.LatestSnapshot,
            GetVersionList(versions.Release), 
            GetVersionList(versions.Snapshot), 
            GetVersionList(versions.Beta), 
            GetVersionList(versions.Alpha));

        _onlineVersions = result;

        return result;
    }
    
    public async Task<Versions> ReadVersionsFromDisk(string directory, CancellationToken cancellationToken)
    {
        var paths = new MinecraftPaths(directory, string.Empty);
        if (!FileManager.DirectoryExist(paths.VersionDirectory))
            return Versions.Empty;
        
        var release = new List<Version>();
        var snapshot = new List<Version>();
        var beta = new List<Version>();
        var alpha = new List<Version>();

        var subDirectories = FileManager.GetSubDirectories(paths.VersionDirectory);
        for (var i = 0; i < subDirectories.Count; i++)
        {
            try
            {
                var subDirectory = subDirectories[i];
                var fileInfos = FileManager.GetFileInfos(subDirectory.FullName);
                var jsonPath = string.Empty;
                for (var j = 0; j < fileInfos.Count; j++)
                {
                    var fileInfo = fileInfos[j];
                    if (fileInfo.Name != $"{subDirectory.Name}.json")
                        continue;
                    
                    jsonPath = fileInfo.FullName;
                    break;
                }

                if (string.IsNullOrEmpty(jsonPath))
                    continue;
                
                var json = await FileManager.ReadFile(jsonPath, cancellationToken);
                if (string.IsNullOrEmpty(json))
                {
                    Logger.Log($"Cant read file: {jsonPath}");
                    continue;
                }
                    
                var minecraftData = _jsonManager.GetMinecraftData(json);
                if (minecraftData == null)
                {
                    Logger.Log($"Cant read json: {jsonPath}");
                    continue;
                }
                
                var type = MinecraftTypeConverter.GetVersionType(minecraftData.MinecraftType);
                var version = new Version(subDirectory.Name, type);

                switch (type)
                {
                    case VersionType.Release:
                        release.Add(version);
                        break;
                    case VersionType.Snapshot:
                        snapshot.Add(version);
                        break;
                    case VersionType.Beta:
                        beta.Add(version);
                        break;
                    case VersionType.Alpha:
                        alpha.Add(version);
                        break;
                    case VersionType.Custom:
                        break;
                    default:
                        Logger.Log($"Unknown minecraft type: {type}");
                        break;
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Log(e);
            }
        }

        var result = new Versions(null, null, release, snapshot, beta, alpha);
        _offlineVersions = result;

        return result;
    }
    
    private static List<Version> GetVersionList(IEnumerable<MinecraftVersion> minecraftVersions)
    {
        var result = new List<Version>();
        foreach (var minecraftVersion in minecraftVersions)
        {
            try
            {
                var type = MinecraftTypeConverter.GetVersionType(minecraftVersion.Type);
                var version = new Version(minecraftVersion.Id, minecraftVersion.Url, type);
                
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