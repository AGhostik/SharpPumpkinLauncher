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

    public override bool Equals(object? obj)
    {
        if (obj is Version version)
            return Equals(version);
        return false;
    }

    private bool Equals(Version other)
    {
        return Id == other.Id && Url == other.Url && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Url, (int)Type);
    }
}