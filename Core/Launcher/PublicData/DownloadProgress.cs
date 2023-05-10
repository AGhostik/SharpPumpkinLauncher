namespace Launcher.PublicData;

public struct DownloadProgress
{
    public DownloadProgress(long bytesReceived, long totalSizeInBytes, float bytesPerSecond, int filesDownloaded, 
        int totalFilesCount)
    {
        BytesReceived = bytesReceived;
        TotalSizeInBytes = totalSizeInBytes;
        FilesDownloaded = filesDownloaded;
        TotalFilesCount = totalFilesCount;
        BytesPerSecond = bytesPerSecond;
    }

    public long BytesReceived { get; }
    public long TotalSizeInBytes { get; }
    public float BytesPerSecond { get; }
    public int FilesDownloaded { get; }
    public int TotalFilesCount { get; }
}