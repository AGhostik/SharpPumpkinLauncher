namespace Launcher.PublicData;

public sealed class Version
{
    public Version(string id, VersionType type)
    {
        Id = id;
        Url = null;
        Type = type;
    }
    
    public Version(string id, string url, VersionType type)
    {
        Id = id;
        Url = url;
        Type = type;
    }

    public string Id { get; }
    public string? Url { get; }
    public VersionType Type { get; }
}