namespace JsonReader;

public sealed class Versions
{
    private readonly List<MinecraftVersion> _release = new();
    private readonly List<MinecraftVersion> _snapshot = new();
    private readonly List<MinecraftVersion> _beta = new();
    private readonly List<MinecraftVersion> _alpha = new();
    
    public string? Latest { get; set; }
    public string? LatestSnapshot { get; set; }

    public IReadOnlyList<MinecraftVersion> Release => _release;

    public IReadOnlyList<MinecraftVersion> Snapshot => _snapshot;

    public IReadOnlyList<MinecraftVersion> Beta => _beta;

    public IReadOnlyList<MinecraftVersion> Alpha => _alpha;
    
    public void AddMinecraftVersion(string id, string url, string sha1, string type)
    {
        var minecraftVersion = new MinecraftVersion(id, url, sha1, type);

        switch (minecraftVersion.Type)
        {
            case MinecraftType.Release:
                _release.Add(minecraftVersion);
                break;
            case MinecraftType.Snapshot:
                _snapshot.Add(minecraftVersion);
                break;
            case MinecraftType.Beta:
                _beta.Add(minecraftVersion);
                break;
            case MinecraftType.Alpha:
                _alpha.Add(minecraftVersion);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}