namespace JsonReader.PublicData.Manifest;

public sealed class MinecraftVersion
{
    private const string Alpha = "old_alpha";
    private const string Beta = "old_beta";
    private const string Release = "release";
    private const string Snapshot = "snapshot";
    
    public MinecraftVersion(string id, string url, string sha1, string type)
    {
        Id = id;
        Url = url;
        Sha1 = sha1;

        Type = type switch
        {
            Release => MinecraftType.Release,
            Snapshot => MinecraftType.Snapshot,
            Beta => MinecraftType.Beta,
            Alpha => MinecraftType.Alpha,
            _ => Type
        };
    }

    public string Id { get; }
    public MinecraftType Type { get; }
    public string Url { get; }
    public string Sha1 { get; }
}