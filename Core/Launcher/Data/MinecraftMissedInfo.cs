namespace Launcher.Data;

public sealed class MinecraftMissedInfo
{
    public long TotalDownloadSize { get; set; }
    public List<string> DirectoriesToCreate { get; } = new();
    public List<(Uri source, string fileName)> DownloadQueue { get; } = new();
    public List<(string fileName, string unpackDirectory)> UnpackItems { get; } = new();
    public List<string> PathsToDelete { get; } = new();
    public List<string> CorruptedFiles { get; } = new();
    public ForgeProfileInstallInfo? ForgeProfileInstallInfo { get; set; }

    public bool IsEmpty => DirectoriesToCreate.Count == 0 &&
                           DownloadQueue.Count == 0 &&
                           UnpackItems.Count == 0 &&
                           PathsToDelete.Count == 0 &&
                           CorruptedFiles.Count == 0 &&
                           ForgeProfileInstallInfo == null;
    
    public bool IsDownloadingNotNeeded => DownloadQueue.Count == 0 &&
                                        CorruptedFiles.Count == 0 &&
                                        ForgeProfileInstallInfo == null;
}