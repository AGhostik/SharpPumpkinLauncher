using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.Json;
using MCLauncher.Json.MinecraftVersionJson;
using MCLauncher.LauncherWindow;
using MCLauncher.Messages;
using MCLauncher.Properties;
using MCLauncher.Tools.Interfaces;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Tools;

public sealed class Installer : IInstaller
{
    private readonly List<Tuple<Uri, string>> _downloadQueue;
    private readonly List<Tuple<string, string[]>> _extractQueue;
    private readonly IFileManager _fileManager;
    private readonly IJsonManager _jsonManager;
    private readonly LaunchArguments _launchArguments;

    private MinecraftVersion? _minecraftVersion;
    private float _progress;

    public Installer(IFileManager fileManager, IJsonManager jsonManager, LaunchArguments launchArguments)
    {
        _fileManager = fileManager;
        _jsonManager = jsonManager;
        _launchArguments = launchArguments;
        _downloadQueue = new List<Tuple<Uri, string>>();
        _extractQueue = new List<Tuple<string, string[]>>();
    }

    public string? LaunchArgs { get; private set; }

    public async Task Install(Profile? profile)
    {
        if (profile == null)
            return;
        
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
        WeakReferenceMessenger.Default.Send(new InstallProgressMessage(100));
    }

    private async Task SetMinecraftVersion(Profile profile)
    {
        var versionPath = $"{profile.GameDirectory}\\versions\\{profile.CurrentVersion}\\{profile.CurrentVersion}";
        _minecraftVersion = await _jsonManager.ParseToObjectAsync<MinecraftVersion>($"{versionPath}.json");
    }

    private void ResetProgress()
    {
        _progress = 0;
        WeakReferenceMessenger.Default.Send(new InstallProgressMessage(0));
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
        WeakReferenceMessenger.Default.Send(new InstallProgressMessage(_progress));
    }

    private void SendProgressText(string? text)
    {
        WeakReferenceMessenger.Default.Send(new StatusMessage(text));
    }

    private void FixProfileDirectoryString(Profile profile)
    {
        profile.GameDirectory = _fileManager.GetPathDirectory(profile.GameDirectory + '\\') ?? string.Empty;
    }

    private async Task CheckAssets(string gameDirectory)
    {
        if (_minecraftVersion?.AssetIndex?.Url == null)
            return;
            
        var assetIndex = await _jsonManager.DownloadJsonAsync(_minecraftVersion.AssetIndex.Url);

        var objects = assetIndex["objects"];
        if (objects == null)
            return;
            
        var assets = objects.Values<JProperty>().ToArray();

        var progressForEach = 4 / assets.Length;

        foreach (var asset in assets)
        {
            if (asset == null)
                continue;
                
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
        var hash = asset.First?["hash"];
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
        var hash = asset.First?["hash"];
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
        if (_minecraftVersion?.Library == null)
            return;
            
        var progressForEach = 4 / _minecraftVersion.Library.Length;
        foreach (var library in _minecraftVersion.Library)
        {
            if (TryGetLibraryInfo(library, out var package, out var name, out var version))
                continue;

            if (!IsLibraryAllow(library))
                continue;

            var url = GetLibraryUrl(library, package, name, version);
            var os = GetLibraryOs(library);

            var savingDirectory = $"{gameDirectory}\\libraries\\{package.Replace('.', '\\')}\\{name}\\{version}";
            var savingFile = $"{savingDirectory}\\{name}-{version}{os}.jar";

            _launchArguments.AddLibrary(savingFile);

            CheckDirectory(savingDirectory);

            if (!_fileManager.FileExist(savingFile))
                AddToDownloadQueue(url, savingFile);

            if (library.Extract?.Exclude != null && library.Extract.Exclude.Length > 0)
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

    private static bool TryGetLibraryInfo(Library library, out string package, out string name, out string version)
    {
        if (library.Name == null)
        {
            package = string.Empty;
            name = string.Empty;
            version = string.Empty;
            return false;
        }
            
        var libraryNameParts = library.Name.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

        if (libraryNameParts.Length < 3)
        {
            package = string.Empty;
            name = string.Empty;
            version = string.Empty;
            return false;
        }
            
        package = libraryNameParts[0];
        name = libraryNameParts[1];
        version = libraryNameParts[2];
        return true;
    }

    private static string GetLibraryUrl(Library library, string package, string name, string version)
    {
        if (library.Downloads != null)
        {
            if (library.Downloads.Artifact?.Url != null)
            {
                return library.Downloads.Artifact.Url;
            }

            if (library.Downloads.Classifiers?.NativesWindows?.Url != null)
            {
                return library.Downloads.Classifiers.NativesWindows.Url;
            }
        }

        return $"{ModelResource.LibrariesUrl}/{package.Replace('.', '/')}/{name}/{version}/{name}-{version}.jar";
    }

    private static string GetLibraryOs(Library library)
    {
        if (library.Natives?.Windows == null)
            return "-natives-windows";
            
        var result = $"-{library.Natives.Windows}";
        if (result.Contains("${arch}"))
        {
            var bit = Environment.Is64BitOperatingSystem ? "64" : "32";
            result = result.Replace("${arch}", bit);
        }

        return result;
    }

    private void ExtractFromQueue(string gameDirectory)
    {
        if (_minecraftVersion == null)
            return;
            
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
            $"{UIResource.CheckVersionFilesStatus_part1} {currentVersion} {UIResource.CheckVersionFilesStatus_part2}");
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