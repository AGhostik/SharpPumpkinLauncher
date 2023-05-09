using Launcher.Data;
using Launcher.PublicData;

namespace Launcher.Tools;

internal sealed class BaseInstaller
{
    public event Action<DownloadProgress>? DownloadingProgress;
    
    public async Task<ErrorCode> RestoreMissedItems(MinecraftMissedInfo missedInfo,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < missedInfo.DirectoriesToCreate.Count; i++)
        {
            var result = FileManager.CreateDirectory(missedInfo.DirectoriesToCreate[i]);
            if (!result)
                return ErrorCode.CreateDirectory;
        }

        for (var i = 0; i < missedInfo.CorruptedFiles.Count; i++)
        {
            var result = FileManager.Delete(missedInfo.CorruptedFiles[i]);
            if (!result)
                return ErrorCode.DeleteFileOrDirectory;
        }

        if (missedInfo.DownloadQueue.Count > 0)
        {
            var result = await DownloadMissingFiles(missedInfo.DownloadQueue, missedInfo.DownloadQueue.Count,
                missedInfo.TotalDownloadSize, cancellationToken);
            if (!result)
                return ErrorCode.Download;
        }

        for (var i = 0; i < missedInfo.UnpackItems.Count; i++)
        {
            var (fileName, destination) = missedInfo.UnpackItems[i];
            var result = FileManager.ExtractToDirectory(fileName, destination);
            if (!result)
                return ErrorCode.ExtractArchive;
        }

        for (var i = 0; i < missedInfo.PathsToDelete.Count; i++)
        {
            var result = FileManager.Delete(missedInfo.PathsToDelete[i]);
            if (!result)
                return ErrorCode.DeleteFileOrDirectory;
        }

        if (missedInfo.AfterInstallTask != null)
        {
            missedInfo.AfterInstallTask.Start();
            var result = await missedInfo.AfterInstallTask.WaitAsync(cancellationToken);
            if (!result)
                return ErrorCode.AfterInstallTask;
        }

        return ErrorCode.NoError;
    }

    private async Task<bool> DownloadMissingFiles(IEnumerable<(Uri source, string fileName)> downloadQueue, 
        int totalFilesCount, long totalSize, CancellationToken cancellationToken)
    {
        var downloadedCount = 0;
        
        DownloadingProgress?.Invoke(new DownloadProgress(0, totalSize, 0, totalFilesCount));
        return await DownloadManager.DownloadFilesParallel(downloadQueue, cancellationToken, BytesReceived,
            FileDownloaded);
        
        void BytesReceived(long bytesReceived)
        {
            DownloadingProgress?.Invoke(
                new DownloadProgress(bytesReceived, totalSize, downloadedCount, totalFilesCount));
        }

        void FileDownloaded()
        {
            downloadedCount++;
        }
    }
}