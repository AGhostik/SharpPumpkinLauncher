using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model.Managers;
using MCLauncher.Model.MinecraftVersionJson;
using MCLauncher.UI;
using MCLauncher.UI.Messages;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model
{
    public class Installer : IInstaller
    {
        private readonly List<Tuple<Uri, string>> _downloadQueue;
        private readonly List<Tuple<string, string[]>> _extractQueue;
        private readonly IFileManager _fileManager;

        private MinecraftVersion _minecraftVersion;
        private float _progress;

        public Installer(IFileManager fileManager)
        {
            _fileManager = fileManager;
            _downloadQueue = new List<Tuple<Uri, string>>();
            _extractQueue = new List<Tuple<string, string[]>>();
        }

        public string LaunchArgs { get; private set; }

        public async Task Install(Profile profile)
        {
            _progress = 0;
            Messenger.Default.Send(new InstallProgressMessage(0));
            _fixProfileDirectoryString(profile);

            _checkDirectories(profile.GameDirectory, profile.CurrentVersion);

            await _checkMinecraftVersion(profile.GameDirectory, profile.CurrentVersion);

            var versionPath = $"{profile.GameDirectory}\\versions\\{profile.CurrentVersion}\\{profile.CurrentVersion}";
            _minecraftVersion = _fileManager.ParseJson<MinecraftVersion>($"{versionPath}.json");

            LaunchArgs = $"{profile.JvmArgs} ";
            LaunchArgs +=
                $"-Djava.library.path=\"{profile.GameDirectory}\\versions\\{profile.CurrentVersion}\\natives\" -cp \"";

            await _checkLibraries(profile.GameDirectory);

            LaunchArgs +=
                $"{profile.GameDirectory}\\versions\\{profile.CurrentVersion}\\{profile.CurrentVersion}.jar\" ";
            LaunchArgs += $"{_minecraftVersion.MainClass} ";
            LaunchArgs += _getMinecraftArguments(profile);

            await _checkAssets(profile.GameDirectory);

            _sendProgressText(UIResource.InstallCompletedStatus);
            Messenger.Default.Send(new InstallProgressMessage(100));
        }

        private void _addProgressAndSend(float value)
        {
            //1 - check directories
            //1 - chechVersions
            //80 - download total // 50 - libraries, 30 - assets
            //8 - checking // 4 - libraries, 4 - assets
            //10 - extract libraries

            if (value <= 0)
                return;
            _progress += value;
            Messenger.Default.Send(new InstallProgressMessage(_progress));
        }

        private void _sendProgressText(string text)
        {
            Messenger.Default.Send(new StatusMessage(text));
        }

        private void _fixProfileDirectoryString(Profile profile)
        {
            profile.GameDirectory = _fileManager.GetPathDirectory(profile.GameDirectory + '\\');
        }

        private string _getMinecraftArguments(Profile profile)
        {
            var args = _minecraftVersion.MinecraftArguments;
            args = args.Replace("${auth_player_name}", profile.Nickname);
            args = args.Replace("${version_name}", profile.CurrentVersion);
            args = args.Replace("${game_directory}", profile.GameDirectory);
            args = args.Replace("${assets_root}", profile.GameDirectory + "assets");
            args = args.Replace("${assets_index_name}", _minecraftVersion.Assets);
            args = args.Replace("${auth_uuid}", _getUuid(profile.Nickname));
            args = args.Replace("${auth_access_token}", "null");
            args = args.Replace("${user_properties}", "{}");
            args = args.Replace("${user_type}", "mojang");
            args = args.Replace("${auth_session}", "null");
            args = args.Replace("${game_assets}", profile.GameDirectory + "\\assets\\virtual\\legacy");

            return args;
        }

        private string _getUuid(string input)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            var guid = new Guid(data);
            return guid.ToString();
        }

        private async Task _checkAssets(string gameDirectory)
        {
            var assetIndex = _fileManager.DownloadJson(_minecraftVersion.AssetIndex.Url);

            var objects = assetIndex["objects"];
            var assets = objects.Values<JProperty>();

            var progressForEach = 4 / assets.Count();

            foreach (var asset in assets)
            {
                var hash = asset.First["hash"].ToString();
                var subDirectory = $"{hash[0]}{hash[1]}";

                if (_minecraftVersion.Assets == "legacy")
                {
                    var legacyFilename = _fileManager.GetPathFilename(asset.Name);

                    var legacySubdirectory = _fileManager.GetPathDirectory(asset.Name);

                    var directory = $"{gameDirectory}\\assets\\virtual\\legacy\\{legacySubdirectory}";

                    _checkDirectory(directory);

                    if (!_fileManager.FileExist($"{directory}\\{legacyFilename}"))
                        _addToDownloadQueue($"{ModelResource.AssetsUrl}/{subDirectory}/{hash}",
                            $"{directory}\\{legacyFilename}");
                }
                else
                {
                    var directory = $"{gameDirectory}\\assets\\objects\\{subDirectory}";

                    _checkDirectory(directory);

                    if (!_fileManager.FileExist($"{directory}\\{hash}"))
                        _addToDownloadQueue($"{ModelResource.AssetsUrl}/{subDirectory}/{hash}", $"{directory}\\{hash}");
                }

                _addProgressAndSend(progressForEach);
            }

            _sendProgressText(UIResource.DownloadAssetsStatus);
            await _downloadFromQueue(50);
        }

        private async Task _checkLibraries(string gameDirectory)
        {
            var progressForEach = 4 / _minecraftVersion.Library.Length;
            foreach (var library in _minecraftVersion.Library)
            {
                if (!_isLibraryAllow(library)) continue;

                var libraryNameParts = library.Name.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                var assembly = libraryNameParts[0];
                var name = libraryNameParts[1];
                var version = libraryNameParts[2];

                var url = string.Empty;

                if (library.Url != null)
                {
                    url = $"{library.Url}{assembly.Replace('.', '/')}/{name}/{version}/{name}-{version}.jar";
                }
                else if (library.Downloads?.Classifiers != null)
                {
                    if (library.Downloads?.Classifiers?.NativesWindows != null)
                        url = library.Downloads.Classifiers.NativesWindows.Url;
                    else if (Environment.Is64BitOperatingSystem &&
                             library.Downloads?.Classifiers?.NativesWindows64 != null)
                        url = library.Downloads.Classifiers.NativesWindows64.Url;
                    else if (!Environment.Is64BitOperatingSystem &&
                             library.Downloads?.Classifiers?.NativesWindows32 != null)
                        url = library.Downloads.Classifiers.NativesWindows32.Url;
                    else if (library.Downloads?.Artifact != null)
                        url = library.Downloads.Artifact.Url;
                }
                else if (library.Downloads?.Artifact != null)
                {
                    url = library.Downloads.Artifact.Url;
                }
                else
                {
                    url =
                        $"{ModelResource.LibrariesUrl}/{assembly.Replace('.', '/')}/{name}/{version}/{name}-{version}.jar";
                }

                var os = string.Empty;
                if (library.Natives != null)
                    if (library.Natives.Windows == "natives-windows-${arch}")
                        os = Environment.Is64BitOperatingSystem ? "-natives-windows-64" : "-natives-windows-32";
                    else
                        os = $"-{library.Natives.Windows}";

                var savingDirectory = $"{gameDirectory}\\libraries\\{assembly.Replace('.', '\\')}\\{name}\\{version}";
                var savingFile = $"{savingDirectory}\\{name}-{version}{os}.jar";

                LaunchArgs += $"{savingFile};";

                _checkDirectory(savingDirectory);

                if (!_fileManager.FileExist(savingFile))
                    _addToDownloadQueue(url, savingFile);

                if (_isLibraryNeedExtract(library))
                {
                    var extractItem = new Tuple<string, string[]>(savingFile, library.Extract.Exclude);
                    _extractQueue.Add(extractItem);
                }

                _addProgressAndSend(progressForEach);
            }

            _sendProgressText(UIResource.DownloadLibrariesStatus);
            await _downloadFromQueue(30);

            _sendProgressText(UIResource.ExtractLibrariesStatus);
            _extractFromQueue(gameDirectory);
        }

        private bool _isLibraryNeedExtract(Library library)
        {
            return library.Extract != null && library.Extract.Exclude.Any();
        }

        private static bool _isLibraryAllow(Library library)
        {
            if (library.Rules == null) return true;

            var allowToAll = false;

            foreach (var rule in library.Rules)
            {
                if (rule.Action == null)
                    continue;

                if (rule.Os == null) allowToAll = rule.Action == "allow";

                if (rule.Action == "disallow" && rule.Os?.Name != null && rule.Os.Name == "windows") return false;
            }

            return allowToAll;
        }

        private void _extractFromQueue(string gameDirectory)
        {
            var progressForEach = 10 / _extractQueue.Count;
            var natives = $"{gameDirectory}\\versions\\{_minecraftVersion.Id}\\natives";
            foreach (var extracTuple in _extractQueue)
            {
                _addProgressAndSend(progressForEach);

                _fileManager.ExtractToDirectory(extracTuple.Item1, natives);

                foreach (var fileOrDirectory in extracTuple.Item2) _fileManager.Delete($"{natives}\\{fileOrDirectory}");
            }
        }

        private async Task _checkMinecraftVersion(string gameDirectory, string currentVersion)
        {
            _sendProgressText(
                $"{UIResource.CheckVersionFIlesStatus_part1} {currentVersion} {UIResource.CheckVersionFIlesStatus_part2}");
            _addProgressAndSend(1);

            var jarFile = $"{gameDirectory}\\versions\\{currentVersion}\\{currentVersion}.jar";
            var jsonFile = $"{gameDirectory}\\versions\\{currentVersion}\\{currentVersion}.json";

            if (!_fileManager.FileExist(jarFile))
                _addToDownloadQueue($"{ModelResource.VersionsDirectoryUrl}/{currentVersion}/{currentVersion}.jar",
                    jarFile);

            if (!_fileManager.FileExist(jsonFile))
                _addToDownloadQueue($"{ModelResource.VersionsDirectoryUrl}/{currentVersion}/{currentVersion}.json",
                    jsonFile);

            await _downloadFromQueue();
        }

        private void _checkDirectories(string gameDirectory, string currentVersion)
        {
            _sendProgressText(UIResource.CheckDirectoriesStatus);
            _addProgressAndSend(1);
            _checkDirectory(gameDirectory);
            _checkDirectory($"{gameDirectory}\\versions\\{currentVersion}\\natives\\");
            _checkDirectory($"{gameDirectory}\\assets\\objects\\");
            _checkDirectory($"{gameDirectory}\\assets\\virtual\\legacy\\");
            _checkDirectory($"{gameDirectory}\\libraries\\");
        }

        private void _checkDirectory(string directory)
        {
            if (!_fileManager.DirectoryExist(directory)) _fileManager.CreateDirectory(directory);
        }

        private async Task _downloadFromQueue(float progressForQueue = 0)
        {
            var progressForEach = progressForQueue / _downloadQueue.Count;

            using (var client = new WebClient())
            {
                foreach (var downloadTuple in _downloadQueue)
                {
                    _addProgressAndSend(progressForEach);
                    await client.DownloadFileTaskAsync(downloadTuple.Item1, downloadTuple.Item2);
                }
            }

            _downloadQueue.Clear();
        }

        private void _addToDownloadQueue(string url, string path)
        {
            if (_fileManager.FileExist(path))
                return;

            _downloadQueue.Add(new Tuple<Uri, string>(new Uri(url), path));
        }
    }
}