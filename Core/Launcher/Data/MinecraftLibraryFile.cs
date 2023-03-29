namespace Launcher.Data;

internal sealed class MinecraftLibraryFile : IMinecraftFile
{
    public MinecraftLibraryFile(string url, int size, string fileName)
    {
        Url = url;
        FileName = fileName;
        Size = size;
    }

    public string Url { get; }
    public string FileName { get; }
    public int Size { get; }
    public bool NeedUnpack { get; init; }
    public IReadOnlyList<string>? Delete { get; init; }
}