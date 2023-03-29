namespace Launcher.Data;

internal sealed class MinecraftFile : IMinecraftFile
{
    public MinecraftFile(string url, int size, string sha1, string fileName)
    {
        Url = url;
        FileName = fileName;
        Sha1 = sha1;
        Size = size;
    }

    public string Url { get; }
    public string FileName { get; }
    public string Sha1 { get; }
    public int Size { get; }
}