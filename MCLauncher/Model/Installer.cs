using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IJsonManager _jsonManager;
        private readonly ILaunchArguments _launchArguments;

        private MinecraftVersion _minecraftVersion;
        private float _progress;

        public Installer(IFileManager fileManager, IJsonManager jsonManager, ILaunchArguments launchArguments)
        {
            _fileManager = fileManager;
            _jsonManager = jsonManager;
            _launchArguments = launchArguments;
            _downloadQueue = new List<Tuple<Uri, string>>();
            _extractQueue = new List<Tuple<string, string[]>>();
        }

        public string LaunchArgs { get; private set; }

        public async Task Install(Profile profile)
        {
            ResetProgress();
            FixProfileDirectoryString(profile);
            CheckDirectories(profile.GameDirectory, profile.CurrentVersion);

            await CheckMinecraftVersion(profile.GameDirectory, profile.CurrentVersion);

            await SetMinecraftVersion(profile);

            await CheckLibraries(profile.GameDirectory);

            SetArgs(profile);

            await CheckAssets(profile.GameDirectory);

            Finish();
        }

        private void SetArgs(Profile profile)
        {
            _launchArguments.Create(profile, _minecraftVersion);
            LaunchArgs = _launchArguments.Get();
        }

        private void Finish()
        {
            SendProgressText(UIResource.InstallCompletedStatus);
            Messenger.Default.Send(new InstallProgressMessage(100));
        }

        private async Task SetMinecraftVersion(Profile profile)
        {
            var versionPath = $"{profile.GameDirectory}\\versions\\{profile.CurrentVersion}\\{profile.CurrentVersion}";
            _minecraftVersion = await _jsonManager.ParseToObjectAsync<MinecraftVersion>($"{versionPath}.json");
        }

        private void ResetProgress()
        {
            _progress = 0;
            Messenger.Default.Send(new InstallProgressMessage(0));
        }

        private void AddProgressAndSend(float value)
        {
            //1% - check directories
            //1% - chechVersions
            //80% - download total // 50% - libraries, 30% - assets
            //8% - checking // 4% - libraries, 4% - assets
            //10% - extract libraries

            if (value <= 0)
                return;
            _progress += value;
            Messenger.Default.Send(new InstallProgressMessage(_progress));
        }

        private void SendProgressText(string text)
        {
            Messenger.Default.Send(new StatusMessage(text));
        }

        private void FixProfileDirectoryString(Profile profile)
        {
            profile.GameDirectory = _fileManager.GetPathDirectory(profile.GameDirectory + '\\');
        }

        private async Task CheckAssets(string gameDirectory)
        {
            var assetIndex = await _jsonManager.DownloadJsonAsync(_minecraftVersion.AssetIndex.Url);

            var objects = assetIndex["objects"];
            if (objects == null)
                return;
            
            var assets = objects.Values<JProperty>().ToArray();

            var progressForEach = 4 / assets.Length;

            foreach (var asset in assets)
            {
                if (_minecraftVersion.Assets == "legacy")
                    AddLegacyAsset(gameDirectory, asset);
                else
                    AddAsset(gameDirectory, asset);

                AddProgressAndSend(progressForEach);
            }

            SendProgressText(UIResource.DownloadAssetsStatus);
            await DownloadFromQueue(50);
        }

        private void AddAsset(string gameDirectory, JToken asset)
        {
            var hash = asset?.First?["hash"];
            if (hash == null)
                return;

            var hashString = hash.ToString();
            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var directory = $"{gameDirectory}\\assets\\objects\\{subDirectory}";

            CheckDirectory(directory);

            if (!_fileManager.FileExist($"{directory}\\{hashString}"))
                AddToDownloadQueue($"{ModelResource.AssetsUrl}/{subDirectory}/{hashString}", $"{directory}\\{hashString}");
        }

        private void AddLegacyAsset(string gameDirectory, JProperty asset)
        {
            var hash = asset?.First?["hash"];
            if (hash == null)
                return;

            var hashString = hash.ToString();

            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var legacyFilename = _fileManager.GetPathFilename(asset.Name);
            var legacySubdirectory = _fileManager.GetPathDirectory(asset.Name);
            var directory = $"{gameDirectory}\\assets\\virtual\\legacy\\{legacySubdirectory}";

            CheckDirectory(directory);

            if (!_fileManager.FileExist($"{directory}\\{legacyFilename}"))
            {
                AddToDownloadQueue($"{ModelResource.AssetsUrl}/{subDirectory}/{hashString}",
                    $"{directory}\\{legacyFilename}");
            }
        }

        private async Task CheckLibraries(string gameDirectory)
        {
            var progressForEach = 4 / _minecraftVersion.Library.Length;
            foreach (var library in _minecraftVersion.Library)
            {
                if (!IsLibraryAllow(library)) continue;

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
                {
                    if (library.Natives.Windows == "natives-windows-${arch}")
                        os = Environment.Is64BitOperatingSystem ? "-natives-windows-64" : "-natives-windows-32";
                    else
                        os = $"-{library.Natives.Windows}";
                }

                var savingDirectory = $"{gameDirectory}\\libraries\\{assembly.Replace('.', '\\')}\\{name}\\{version}";
                var savingFile = $"{savingDirectory}\\{name}-{version}{os}.jar";

                _launchArguments.AddLibrary(savingFile);

                CheckDirectory(savingDirectory);

                if (!_fileManager.FileExist(savingFile))
                    AddToDownloadQueue(url, savingFile);

                if (IsLibraryNeedExtract(library))
                {
                    var extractItem = new Tuple<string, string[]>(savingFile, library.Extract.Exclude);
                    _extractQueue.Add(extractItem);
                }

                AddProgressAndSend(progressForEach);
            }

            SendProgressText(UIResource.DownloadLibrariesStatus);
            await DownloadFromQueue(30);

            SendProgressText(UIResource.ExtractLibrariesStatus);
            ExtractFromQueue(gameDirectory);
        }

        private bool IsLibraryNeedExtract(Library library)
        {
            return library.Extract != null && library.Extract.Exclude.Any();
        }

        private static bool IsLibraryAllow(Library library)
        {
            if (library.Rules == null) return true;

            var allowToAll = false;

            foreach (var rule in library.Rules)
            {
                if (rule.Action == null)
                    continue;

                if (rule.Os == null)
                    allowToAll = rule.Action == "allow";

                if (rule.Action == "disallow" && rule.Os?.Name != null && rule.Os.Name == "windows")
                    return false;
            }

            return allowToAll;
        }

        private void ExtractFromQueue(string gameDirectory)
        {
            var progressForEach = 10 / _extractQueue.Count;
            var natives = $"{gameDirectory}\\versions\\{_minecraftVersion.Id}\\natives";
            foreach (var extracTuple in _extractQueue)
            {
                AddProgressAndSend(progressForEach);

                _fileManager.ExtractToDirectory(extracTuple.Item1, natives);

                foreach (var fileOrDirectory in extracTuple.Item2)
                    _fileManager.Delete($"{natives}\\{fileOrDirectory}");
            }
        }

        private async Task CheckMinecraftVersion(string gameDirectory, string currentVersion)
        {
            SendProgressText(
                $"{UIResource.CheckVersionFIlesStatus_part1} {currentVersion} {UIResource.CheckVersionFIlesStatus_part2}");
            AddProgressAndSend(1);

            CheckMinecraftVersionFile(gameDirectory, currentVersion, "jar");
            CheckMinecraftVersionFile(gameDirectory, currentVersion, "json");

            await DownloadFromQueue();
        }

        private void CheckMinecraftVersionFile(string gameDirectory, string currentVersion, string fileType)
        {
            var jarFile = $"{gameDirectory}\\versions\\{currentVersion}\\{currentVersion}.{fileType}";

            if (!_fileManager.FileExist(jarFile))
            {
                AddToDownloadQueue($"{ModelResource.VersionsDirectoryUrl}/{currentVersion}/{currentVersion}.{fileType}",
                    jarFile);
            }
        }

        private void CheckDirectories(string gameDirectory, string currentVersion)
        {
            SendProgressText(UIResource.CheckDirectoriesStatus);
            AddProgressAndSend(1);

            CheckDirectory(gameDirectory);
            CheckDirectory($"{gameDirectory}\\versions\\{currentVersion}\\natives\\");
            CheckDirectory($"{gameDirectory}\\assets\\objects\\");
            CheckDirectory($"{gameDirectory}\\assets\\virtual\\legacy\\");
            CheckDirectory($"{gameDirectory}\\libraries\\");
        }

        private void CheckDirectory(string directory)
        {
            if (!_fileManager.DirectoryExist(directory))
                _fileManager.CreateDirectory(directory);
        }

        private async Task DownloadFromQueue(float progressForQueue = 0)
        {
            if (!_downloadQueue.Any())
            {
                AddProgressAndSend(progressForQueue);
                return;
            }

            var progressForEach = progressForQueue / _downloadQueue.Count;

            await _fileManager.DownloadFiles(_downloadQueue, () => { AddProgressAndSend(progressForEach); });

            _downloadQueue.Clear();
        }

        private void AddToDownloadQueue(string url, string path)
        {
            if (_fileManager.FileExist(path))
                return;

            _downloadQueue.Add(new Tuple<Uri, string>(new Uri(url), path));
        }
    }
}