using JsonReader;
using Launcher.Tools;
using ForgeVersion = Launcher.PublicData.ForgeVersion;
using ForgeVersions = Launcher.PublicData.ForgeVersions;

namespace Launcher.Forge;

internal sealed class ForgeVersionsLoader
{
    private readonly Dictionary<string, ForgeVersions> _forgeVersions = new();
    private readonly JsonManager _jsonManager;
    
    public ForgeVersionsLoader(JsonManager jsonManager)
    {
        _jsonManager = jsonManager;
    }
    public async Task<ForgeVersion?> GetForgeVersion(string versionId, string forgeVersionId, 
        CancellationToken cancellationToken = default)
    {
        if (_forgeVersions.TryGetValue(versionId, out var forgeVersions))
        {
            if (forgeVersions.AllForgeVersions.TryGetValue(forgeVersionId, out var forgeVersion))
                return forgeVersion;
        }

        var loadedForgeVersions = await GetOnlineForgeVersions(versionId, cancellationToken);

        if (loadedForgeVersions.AllForgeVersions.TryGetValue(forgeVersionId, out var loadedForgeVersion))
            return loadedForgeVersion;

        return null;
    }

    public async Task<ForgeVersions> GetOnlineForgeVersions(string versionId, CancellationToken cancellationToken)
    {
        if (_forgeVersions.TryGetValue(versionId, out var alreadyLoadedForgeVersions))
            return alreadyLoadedForgeVersions;

        var parameters = new Dictionary<string, string>
        {
            { "version", versionId },
            { "includeAll", "true" }
        };

        var forgeVersionsJson = await DownloadManager.GetRequest(WellKnownUrls.CurseForge, parameters, cancellationToken);
        var forgeVersions = _jsonManager.GetForgeVersions(forgeVersionsJson, WellKnownUrls.CurseForge);
        
        if (forgeVersions == null)
            return ForgeVersions.Empty;
        
        ForgeVersion? recommended = null;
        ForgeVersion? latest = null;
        var versions = new List<ForgeVersion>(forgeVersions.Versions.Count);
        for (var i = 0; i < forgeVersions.Versions.Count; i++)
        {
            var version = forgeVersions.Versions[i];

            var isLatest = version.Id == forgeVersions.Latest?.Id;
            var isRecommended = version.Id == forgeVersions.Recommended?.Id;
            
            var forgeVersion = new ForgeVersion(version.Id, version.Url, version.MinecraftId)
            {
                IsLatest = isLatest,
                IsRecommended = isRecommended
            };
            
            if (isLatest)
                latest = forgeVersion;
            
            if (isRecommended)
                recommended = forgeVersion;
            
            versions.Add(forgeVersion);
        }
        
        versions.Sort((versionA, versionB) => versionB.CompareTo(versionA));
        
        var result = new ForgeVersions(latest, recommended, versions);
        _forgeVersions.Add(versionId, result);
        return result;
    }

}