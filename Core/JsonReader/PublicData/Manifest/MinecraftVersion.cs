using JsonReader.Tools;

namespace JsonReader.PublicData.Manifest;

public sealed class MinecraftVersion
{
    public MinecraftVersion(string id, string url, string sha1, string type)
    {
        Id = id;
        Url = url;
        Sha1 = sha1;
        Type = MinecraftTypeConverter.GetMinecraftType(type);
    }

    public string Id { get; }
    public MinecraftType Type { get; }
    public string Url { get; }
    public string Sha1 { get; }
}