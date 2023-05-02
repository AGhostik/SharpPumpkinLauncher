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
    public VersionType Type { get; }
    public string? Url { get; }
    
    public static Version? Merge(Version a, Version b)
    {
        if (a.Id != b.Id || a.Type != b.Type)
            return null;

        var url = string.IsNullOrEmpty(a.Url) ? b.Url : a.Url;
        
        if (string.IsNullOrEmpty(url))
            return new Version(a.Id, a.Type);

        return new Version(a.Id, url, a.Type);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Version version)
            return Equals(version);
        return false;
    }

    public bool Equals(Version? other)
    {
        if (other == null)
            return false;
        
        return Id == other.Id && Type == other.Type && Url == other.Url;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int)Type, Url);
    }
}