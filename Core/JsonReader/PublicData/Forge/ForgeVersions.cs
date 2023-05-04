namespace JsonReader.PublicData.Forge;

public sealed class ForgeVersions
{
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