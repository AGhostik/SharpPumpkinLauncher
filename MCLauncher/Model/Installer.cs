using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MCLauncher.Model.MinecraftVersionJson;

namespace MCLauncher.Model
{
    public class Installer
    {
        private readonly FileManager _fileManager;
        private readonly List<Tuple<Uri, string>> _downloadQueue;
        private readonly List<Tuple<string, string[]>> _extractQueue;

        public Installer(FileManager fileManager)
        {
            _fileManager = fileManager;
            _downloadQueue = new List<Tuple<Uri, string>>();
            _extractQueue = new List<Tuple<string, string[]>>();
        }

        public async void Install(Profile profile)
        {
            _checkDirectories(profile.GameDirectory, profile.CurrentVersion);

            //checking profile fields

            await _checkMinecraftVersion(profile.GameDirectory, profile.CurrentVersion);

            await _checkLibraries(profile.GameDirectory, profile.CurrentVersion);
        }

        private async Task _checkLibraries(string gameDirectory, string currentVersion)
        {
            var versionPath = $"{gameDirectory}versions\\{currentVersion}\\{currentVersion}";
            var minecraftVersion = _fileManager.ParseJson<MinecraftVersion>($"{versionPath}.json");
            foreach (var library in minecraftVersion.Libraries)
            {
                if (!_isLibraryAllow(library))
                {
                    continue;
                }

                //
                var libraryNameParts = library.Name.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                var assembly = libraryNameParts[0];
                var name = libraryNameParts[1];
                var version = libraryNameParts[2];

                var url = $"{ModelResource.LibrariesUrl}{assembly.Replace('.', '/')}/{name}/{version}/{name}-{version}.jar";
                var os = string.Empty;
                if (library.Natives != null)
                {
                    if (library.Natives.Windows == "natives-windows-${arch}")
                    {
                        os = Environment.Is64BitOperatingSystem ? "-natives-windows-64" : "-natives-windows-32";
                    }
                    else
                    {
                        os = $"-{library.Natives.Windows}";
                    }
                }

                var savingDirectory = $"{gameDirectory}libraries\\{assembly.Replace('.', '\\')}\\{name}\\{version}\\";
                var savingFile = $"{savingDirectory}{name}-{version}{os}.jar";

                _checkDirectory(savingDirectory);
                _addToDownloadQueue(url, savingFile);

                _chechLibraryExtract(library, savingFile);
                //
            }

            await _downloadFromQueue();
            _extractFromQueue(gameDirectory, currentVersion);
        }

        private void _chechLibraryExtract(Libraries library, string savingFile)
        {
            if (library.Extract == null || !library.Extract.Exclude.Any()) return;

            var extractItem = new Tuple<string, string[]>(savingFile, library.Extract.Exclude);
            _extractQueue.Add(extractItem);
        }

        private static bool _isLibraryAllow(Libraries library)
        {
            if (library.Rules == null) return true;

            foreach (var rule in library.Rules)
            {
                if (rule.Action != null && rule.Action == "disallow" && rule.Os?.Name != null && rule.Os.Name == "windows") 
                    return false;
            }

            return true;
        }

        private void _extractFromQueue(string gameDirectory, string currentVersion)
        {
            foreach (var extracTuple in _extractQueue)
            {
                _fileManager.ExtractToDirectory(extracTuple.Item1, $"{gameDirectory}versions\\{currentVersion}\\natives");
                foreach (var fileOrDirectory in extracTuple.Item2)
                {
                    _fileManager.Delete(fileOrDirectory);
                }
            }
        }

        private async Task _checkMinecraftVersion(string gameDirectory, string currentVersion)
        {
            var versionFilePath = $"{gameDirectory}versions\\{currentVersion}\\{currentVersion}";
            if (!_fileManager.FileExist($"{versionFilePath}.jar"))
            {
                _addToDownloadQueue($"{ModelResource.VersionsDirectoryUrl}{currentVersion}/{currentVersion}.jar",
                    $"{versionFilePath}.jar");
            }
            if (!_fileManager.FileExist($"{versionFilePath}.json"))
            {
                _addToDownloadQueue($"{ModelResource.VersionsDirectoryUrl}{currentVersion}/{currentVersion}.json",
                    $"{versionFilePath}.json");
            }
            await _downloadFromQueue();
        }

        private void _checkDirectories(string gameDirectory, string currentVersion)
        {
            _checkDirectory(gameDirectory);
            _checkDirectory($"{gameDirectory}versions\\{currentVersion}\\natives");
            _checkDirectory($"{gameDirectory}assets\\indexes");
            _checkDirectory($"{gameDirectory}assets\\objects");
            _checkDirectory($"{gameDirectory}libraries");
        }

        private void _checkDirectory(string directory)
        {
            if (!_fileManager.DirectoryExist(directory))
            {
                _fileManager.CreateDirectory(directory);
            }
        }

        private async Task _downloadFromQueue()
        {
            using (var client = new WebClient())
            {
                //client.DownloadFileCompleted += _fileDownloaded;
                foreach (var downloadTuple in _downloadQueue)
                {
                    await client.DownloadFileTaskAsync(downloadTuple.Item1, downloadTuple.Item2);
                }
            }
            _downloadQueue.Clear();
        }

        private void _fileDownloaded(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            //
            // percent = downloadedCount / _downloadQueue.Count
            // Invoke(percent);
            //
        }

        private void _addToDownloadQueue(string url, string path)
        {
            _downloadQueue.Add(new Tuple<Uri, string>(new Uri(url), path));
        }
    }
}