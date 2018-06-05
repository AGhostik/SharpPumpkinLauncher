using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Ionic.Zip;
using Microsoft.Win32;

namespace MCLauncher.Model.Managers
{
    public class FileManager : IFileManager
    {
        public void StartProcess(string fileName, string args, Action exitedAction)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(fileName, args),
                EnableRaisingEvents = true
            };

            process.Exited += (sender, eventArgs) =>
            {
                exitedAction.Invoke();
                Debug.WriteLine(process.ExitCode);
            };

            process.Start();
        }

        public string GetPathFilename(string source)
        {
            return Path.GetFileName(source);
        }

        public string GetPathDirectory(string source)
        {
            return Path.GetDirectoryName(source);
        }

        public void ExtractToDirectory(string sourceArchive, string destinationDirectory)
        {
            using (var zip = ZipFile.Read(sourceArchive))
            {
                foreach (var entry in zip)
                    entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            if (FileExist(path)) File.Delete(path);

            if (DirectoryExist(path)) Directory.Delete(path, true);
        }

        public bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public void StartProcess(string fileName)
        {
            Process.Start(fileName);
        }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public string GetJavawPath()
        {
            string java;

            using (var baseKey = Registry.LocalMachine.OpenSubKey(ModelResource.JavaKey))
            {
                if (baseKey == null) return string.Empty;

                var currentVersion = baseKey.GetValue("CurrentVersion").ToString();
                using (var homeKey = Registry.LocalMachine.OpenSubKey($"{ModelResource.JavaKey}\\{currentVersion}"))
                {
                    if (homeKey == null) return string.Empty;

                    java = $"{homeKey.GetValue("JavaHome")}\\bin\\javaw.exe";
                }
            }

            return java;
        }

        public async Task DownloadFile(string url, string fileName)
        {
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(url, fileName);
            }
        }

        public async Task DownloadFile(List<Tuple<Uri, string>> urlFileName, Action downloadedEvent = null)
        {
            using (var client = new WebClient())
            {
                if (downloadedEvent != null) client.DownloadFileCompleted += (sender, args) => { downloadedEvent(); };

                foreach (var tuple in urlFileName) await client.DownloadFileTaskAsync(tuple.Item1, tuple.Item2);
            }
        }
    }
}