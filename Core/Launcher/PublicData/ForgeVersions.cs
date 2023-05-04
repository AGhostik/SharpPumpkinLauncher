namespace Launcher.PublicData;

public sealed class ForgeVersions
{
    private readonly Dictionary<string, ForgeVersion> _forgeVersions;
    public static ForgeVersions Empty { get; } = new();

    private ForgeVersions()
    {
        Latest = null;
        Recommended = null;
        Versions = Array.Empty<ForgeVersion>();
        _forgeVersions = new Dictionary<string, ForgeVersion>();
    }
    
    public ForgeVersions(ForgeVersion? latest, ForgeVersion? recommended, IReadOnlyList<ForgeVersion> versions)
    {
        Versions = versions;
        Latest = latest;
        Recommended = recommended;

        _forgeVersions = new Dictionary<string, ForgeVersion>();
        for (var i = 0; i < versions.Count; i++)
            _forgeVersions.Add(versions[i].Id, versions[i]);
    }
    
    public IReadOnlyDictionary<string, ForgeVersion> AllForgeVersions => _forgeVersions;

    public ForgeVersion? Latest { get; }
    public ForgeVersion? Recommended { get; }
    public IReadOnlyList<ForgeVersion> Versions { get; }
}