namespace Launcher.PublicData;

public sealed class ForgeVersions
{
    public static ForgeVersions Empty { get; } = new();

    private ForgeVersions()
    {
        Latest = null;
        Recommended = null;
        Versions = Array.Empty<ForgeVersion>();
    }
    
    public ForgeVersions(ForgeVersion? latest, ForgeVersion? recommended, IReadOnlyList<ForgeVersion> versions)
    {
        Versions = versions;
        Latest = latest;
        Recommended = recommended;
    }

    public ForgeVersion? Latest { get; }
    public ForgeVersion? Recommended { get; }
    public IReadOnlyList<ForgeVersion> Versions { get; }
}