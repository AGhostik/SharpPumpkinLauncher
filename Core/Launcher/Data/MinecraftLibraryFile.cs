namespace Launcher.Data;

internal sealed class MinecraftLibraryFile : IMinecraftFile
{
    public MinecraftLibraryFile(string url, string fileName)
    {
        Url = url;
        FileName = fileName;
    }

    public string Url { get; }
    public string FileName { get; }
    public bool NeedUnpack { get; set; }
    public List<string> Delete { get; } = new();
}