namespace JsonReader.PublicData.Runtime;

public sealed class Runtime
{
    public Runtime(string name, DateTime released, string sha1, int size, string url)
    {
        Name = name;
        Released = released;
        Sha1 = sha1;
        Size = size;
        Url = url;
    }

    public string Name { get; }
    public DateTime Released { get; }
    public string Sha1 { get; }
    public int Size { get; }
    public string Url { get; }
}