namespace Launcher.PublicData;

public sealed class Version
{
    public Version(string id, VersionType type)
    {
        Id = id;
        Type = type;
    }

    public string Id { get; }

    public VersionType Type { get; }
}