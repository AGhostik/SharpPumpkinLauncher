namespace JsonReader.PublicData.Assets;

public sealed class Asset
{
    public Asset(string name, string hash, int size)
    {
        Name = name;
        Hash = hash;
        Size = size;
    }

    public string Name { get; }
    public string Hash { get; }
    public int Size { get; }
}