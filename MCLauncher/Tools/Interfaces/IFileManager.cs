using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCLauncher.Tools.Interfaces;

public interface IFileManager
{
    void CreateDirectory(string directory);
    void Delete(string path);
    bool DirectoryExist(string path);
    Task DownloadFiles(List<Tuple<Uri, string>> urlFileName, Action? downloadedEvent = null);
    void ExtractToDirectory(string sourceArchive, string destinationDirectory);
    bool FileExist(string path);
    string GetJavawPath();
    string? GetPathDirectory(string source);
    string GetPathFilename(string source);
    void StartProcess(string fileName);
    void StartProcess(string fileName, string? args, Action? exitedAction);
}