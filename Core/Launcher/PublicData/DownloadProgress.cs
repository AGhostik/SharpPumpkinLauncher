namespace Launcher.PublicData;

public struct DownloadProgress
{
    public DownloadProgress(long bytesReceived, long totalSizeInBytes, int filesDownloaded, int totalFilesCount)
    {
        BytesReceived = bytesReceived;
        TotalSizeInBytes = totalSizeInBytes;
        FilesDownloaded = filesDownloaded;
        TotalFilesCount = totalFilesCount;
    }

    public long BytesReceived { get; }
    public long TotalSizeInBytes { get; }
    public int FilesDownloaded { get; }
    public int TotalFilesCount { get; }
}