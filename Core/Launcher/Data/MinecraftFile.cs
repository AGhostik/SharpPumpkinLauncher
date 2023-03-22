namespace Launcher.Data;

internal sealed class MinecraftFile : IMinecraftFile
{
    public MinecraftFile(string url, string fileName)
    {
        Url = url;
        FileName = fileName;
    }

    public string Url { get; }
    public string FileName { get; }
}