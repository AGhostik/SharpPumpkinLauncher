namespace Launcher.PublicData;

public sealed class ForgeVersion : IComparable<ForgeVersion>
{
    public ForgeVersion(string id, string url, string minecraftId)
    {
        Id = id;
        Url = url;
        MinecraftId = minecraftId;
    }

    public string Id { get; }
    public string Url { get; }
    public string MinecraftId { get; }
    
    public bool IsRecommended { get; init; }
    public bool IsLatest { get; init; }

    public int CompareTo(ForgeVersion? other)
    {
        if (ReferenceEquals(this, other) || other == null)
            return 0;

        if (Id.Length > other.Id.Length)
            return 1;
        if (Id.Length < other.Id.Length)
            return -1;

        return string.Compare(Id, other.Id, StringComparison.Ordinal);
    }
}