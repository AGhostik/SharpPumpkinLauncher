namespace JsonReader.PublicData.Game;

public sealed class DownloadFile
{
    public DownloadFile(string sha1, int size, string url)
    {
        Sha1 = sha1;
        Size = size;
        Url = url;
    }

    public string Sha1 { get; }
    public int Size { get; }
    public string Url { get; }
}