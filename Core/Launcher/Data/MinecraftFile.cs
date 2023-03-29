namespace Launcher.Data;

internal sealed class MinecraftFile : IMinecraftFile
{
    public MinecraftFile(string url, int size, string fileName)
    {
        Url = url;
        FileName = fileName;
        Size = size;
    }

    public string Url { get; }
    public string FileName { get; }
    public int Size { get; }
}