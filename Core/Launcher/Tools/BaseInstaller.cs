using System.Diagnostics;
using Launcher.Data;
using Launcher.Forge;
using Launcher.PublicData;

namespace Launcher.Tools;

internal sealed class BaseInstaller
{
    public event Action<DownloadProgress>? DownloadingProgress;
    
    public async Task<ErrorCode> RestoreMissedItems(MinecraftMissedInfo missedInfo, 
        ForgeProfileInstaller forgeProfileInstaller, CancellationToken cancellationToken = default)
    {
        var result = await RestoreMissedItems(missedInfo, cancellationToken);
        if (result != ErrorCode.NoError)
            return result;

        if (missedInfo.ForgeProfileInstallInfo != null)
        {
            var installResult = await forgeProfileInstaller.Install(missedInfo.ForgeProfileInstallInfo.ForgeInstall, 
                missedInfo.ForgeProfileInstallInfo.JavaFile, missedInfo.ForgeProfileInstallInfo.MinecraftJar,
                missedInfo.ForgeProfileInstallInfo.LibrariesPath, cancellationToken);
            
            if (!installResult)
                return ErrorCode.AfterInstallTask;
        }

        return ErrorCode.NoError;
    }
    
    public async Task<ErrorCode> RestoreMissedItems(MinecraftMissedInfo missedInfo,
        CancellationToken cancellationToken = default)
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

        return ErrorCode.NoError;
    }

    private async Task<bool> DownloadMissingFiles(IEnumerable<(Uri source, string fileName)> downloadQueue, 
        int totalFilesCount, long totalSize, CancellationToken cancellationToken)
    {
        var downloadedCount = 0;
        var bytesPerSecond = 0f;
        var bytesReceivedPrevious = 0L;
        var stopwatch = new Stopwatch();
        
        DownloadingProgress?.Invoke(new DownloadProgress(0, totalSize, 0, 0, totalFilesCount));
        stopwatch.Start();
        var result = 
            await DownloadManager.DownloadFilesParallel(downloadQueue, BytesReceived, FileDownloaded, cancellationToken);
        stopwatch.Stop();
        return result;
        
        void BytesReceived(long bytesReceived)
        {
            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                bytesPerSecond = (bytesReceived - bytesReceivedPrevious) / (stopwatch.ElapsedMilliseconds / 1000f);
                stopwatch.Restart();
                bytesReceivedPrevious = bytesReceived;
            }

            DownloadingProgress?.Invoke(
                new DownloadProgress(bytesReceived, totalSize, bytesPerSecond, downloadedCount, totalFilesCount));
        }

        void FileDownloaded()
        {
            downloadedCount++;
        }
    }
}