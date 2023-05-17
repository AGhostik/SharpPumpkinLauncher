namespace Launcher.Data;

public sealed class MinecraftMissedInfo
{
    public MinecraftMissedInfo(long totalDownloadSize, IReadOnlyList<string> directoriesToCreate, 
        IReadOnlyList<(Uri source, string fileName)> downloadQueue, 
        IReadOnlyList<(string fileName, string unpackDirectory)> unpackItems, 
        IReadOnlyList<string> pathsToDelete, IReadOnlyList<string> corruptedFiles, 
        ForgeProfileInstallInfo? forgeProfileInstallInfo = null)
    {
        TotalDownloadSize = totalDownloadSize;
        DirectoriesToCreate = directoriesToCreate;
        DownloadQueue = downloadQueue;
        UnpackItems = unpackItems;
        PathsToDelete = pathsToDelete;
        CorruptedFiles = corruptedFiles;
        ForgeProfileInstallInfo = forgeProfileInstallInfo;
    }

    public long TotalDownloadSize { get; }
    public IReadOnlyList<string> DirectoriesToCreate { get; }
    public IReadOnlyList<(Uri source, string fileName)> DownloadQueue { get; }
    public IReadOnlyList<(string fileName, string unpackDirectory)> UnpackItems { get; }
    public IReadOnlyList<string> PathsToDelete { get; }
    public IReadOnlyList<string> CorruptedFiles { get; }
    public ForgeProfileInstallInfo? ForgeProfileInstallInfo { get; }

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