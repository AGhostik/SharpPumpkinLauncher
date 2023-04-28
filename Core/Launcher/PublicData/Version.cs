namespace Launcher.PublicData;

public sealed class Version
{
    public Version(string id, VersionType type)
    {
        Id = id;
        Url = null;
        Type = type;
        IsInstalled = true;
    }
    
    public Version(string id, string url, VersionType type)
    {
        Id = id;
        Url = url;
        Type = type;
    }

    public string Id { get; }
    public VersionType Type { get; }
    public string? Url { get; private set; }
    public bool IsInstalled { get; private set; }

    public bool Merge(Version from)
    {
        if (Id != from.Id || Type != from.Type)
            return false;

        if (string.IsNullOrEmpty(Url))
            Url = from.Url;

        if (!IsInstalled)
            IsInstalled = from.IsInstalled;

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Version version)
            return Equals(version);
        return false;
    }

    private bool Equals(Version other)
    {
        return Id == other.Id && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int)Type);
    }
}