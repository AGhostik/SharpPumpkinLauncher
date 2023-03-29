namespace Launcher.Data;

public sealed class MinecraftMissedInfo
{
    public long TotalDownloadSize { get; set; }
    public List<string> DirectoriesToCreate { get; } = new();
    public List<(Uri source, string fileName)> DownloadQueue { get; } = new();
    public List<(string fileName, string unpackDirectory)> UnpackItems { get; } = new();
    public List<string> PathsToDelete { get; } = new();
    public List<string> CorruptedFiles { get; } = new();
}