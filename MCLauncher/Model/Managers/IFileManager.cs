using System;

namespace MCLauncher.Model.Managers
{
    public interface IFileManager
    {
        void CreateDirectory(string directory);
        void Delete(string path);
        bool DirectoryExist(string path);
        void ExtractToDirectory(string sourceArchive, string destinationDirectory);
        bool FileExist(string path);
        string FindJava();
        string GetPathDirectory(string source);
        string GetPathFilename(string source);
        void StartProcess(string fileName);
        void StartProcess(string fileName, string args, Action exitedAction);
    }
}