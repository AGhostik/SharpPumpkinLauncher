namespace JsonReader.PublicData.Forge;

public sealed class ForgeVersion
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
}