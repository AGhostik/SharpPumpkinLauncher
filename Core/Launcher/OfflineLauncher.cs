using JsonReader;
using Launcher.PublicData;
using Launcher.Tools;
using Version = Launcher.PublicData.Version;

namespace Launcher;

internal sealed class OfflineLauncher : ILauncher
{
    private readonly JsonManager _jsonManager;
    public event Action<LaunchProgress, float>? LaunchMinecraftProgress;
    
    public OfflineLauncher()
    {
        _jsonManager = new JsonManager();
    }
    
    public async Task<Versions> GetAvailableVersions(string directory, CancellationToken cancellationToken)
    {
        var paths = new MinecraftPaths(directory, string.Empty);
        if (!FileManager.DirectoryExist(paths.VersionDirectory))
            return Versions.Empty;
        
        var release = new List<Version>();
        var snapshot = new List<Version>();
        var beta = new List<Version>();
        var alpha = new List<Version>();

        var subDirectories = FileManager.GetSubDirectories(paths.VersionDirectory);
        for (var i = 0; i < subDirectories.Count; i++)
        {
            var subDirectory = subDirectories[i];
            var fileInfos = FileManager.GetFileInfos(subDirectory.FullName);
            var hasJar = false;
            var jsonPath = string.Empty;
            for (var j = 0; j < fileInfos.Count; j++)
            {
                var fileInfo = fileInfos[j];
                if (fileInfo.Name == $"{subDirectory.Name}.jar")
                {
                    hasJar = true;
                }
                else if (fileInfo.Name == $"{subDirectory.Name}.json")
                {
                    jsonPath = fileInfo.FullName;
                }
            }

            if (!hasJar || string.IsNullOrEmpty(jsonPath))
                continue;
            
            var json = await FileManager.ReadFile(jsonPath, cancellationToken);
            if (string.IsNullOrEmpty(json))
                continue;
                
            var minecraftData = _jsonManager.GetMinecraftData(json);
            if (minecraftData == null)
                continue;
            
            //todo: write to dictionary minecraftData

            var type = MinecraftTypeConverter.GetVersionType(minecraftData.MinecraftType);
            var version = new Version(minecraftData.Id, type);

            switch (type)
            {
                case VersionType.Release:
                    release.Add(version);
                    break;
                case VersionType.Snapshot:
                    snapshot.Add(version);
                    break;
                case VersionType.Beta:
                    beta.Add(version);
                    break;
                case VersionType.Alpha:
                    alpha.Add(version);
                    break;
                case VersionType.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return new Versions(null, null, release, snapshot, beta, alpha);
    }

    public async Task LaunchMinecraft(LaunchData launchData, CancellationToken cancellationToken,
        Action? exitedAction = null)
    {
        try
        {
            LaunchMinecraftProgress?.Invoke(LaunchProgress.GetVersionData, 0f);
            
            var minecraftPaths = new MinecraftPaths(launchData.GameDirectory, launchData.Version.Id);

            var minecraftVersionJson =
                await FileManager.ReadFile($"{minecraftPaths.VersionDirectory}\\{launchData.Version.Id}.json",
                    cancellationToken);

            var minecraftData = _jsonManager.GetMinecraftData(minecraftVersionJson);

            if (minecraftData == null)
                return;

            var assetsDataJson = await FileManager.ReadFile(
                $"{minecraftPaths.AssetsIndexDirectory}\\{minecraftData.AssetsVersion}.json",
                cancellationToken);
            var assetsData = _jsonManager.GetAssets(assetsDataJson);

            if (assetsData == null)
                return;

            var fileList = FileManager.GetFileList(minecraftData, assetsData, minecraftPaths, minecraftData.Id);

            var launchArgumentsData =
                new LaunchArgumentsData(minecraftData, fileList, minecraftPaths, launchData.PlayerName);
            var launchArguments = LaunchArgumentsBuilder.GetLaunchArguments(minecraftData, launchArgumentsData);

            LaunchMinecraftProgress?.Invoke(LaunchProgress.StartGame, 0f);
            await FileManager.StartProcess("java", launchArguments, exitedAction);
        }
        catch (TaskCanceledException e)
        {
            Console.WriteLine(e);
            LaunchMinecraftProgress?.Invoke(LaunchProgress.GameAborted, 0f);
        }
    }
}