using JsonReader.PublicData.Game;
using Launcher.Data;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsData
{
    public LaunchArgumentsData(MinecraftData minecraftData, MinecraftFileList fileList, MinecraftPaths minecraftPaths,
        string playerName)
    {
        VersionId = minecraftData.Id;
        VersionType = minecraftData.Type;
        ClientJar = FileManager.GetFullPath(fileList.Client.FileName);
        PlayerName = playerName;

        AssetsVersion = minecraftData.AssetsVersion;

        GameDirectory = FileManager.GetFullPath(minecraftPaths.GameDirectory);
        AssetsDirectory = FileManager.GetFullPath(minecraftPaths.AssetsDirectory);
        LibrariesDirectory = FileManager.GetFullPath(minecraftPaths.LibrariesDirectory);
        NativesDirectory = FileManager.GetFullPath(minecraftPaths.NativesDirectory);

        LoggingArgument = minecraftData.LoggingData?.Argument ?? "null";
        LoggingFile = fileList.Logging == null ? "null" : $"\"{FileManager.GetFullPath(fileList.Logging.FileName)}\"";

        var lib = new string[fileList.LibraryFiles.Count];
        for (var i = 0; i < fileList.LibraryFiles.Count; i++)
            lib[i] = FileManager.GetFullPath(fileList.LibraryFiles[i].FileName);

        Libraries = lib;
    }

    public string LauncherName => "\"mclauncher\"";

    public string VersionId { get; }
    public string VersionType { get; }
    public string ClientJar { get; }
    
    public string PlayerName { get; }
    
    public string AssetsVersion { get; }
        
    public string GameDirectory { get; }
    public string AssetsDirectory { get; }
    public string LibrariesDirectory { get; }
    public string NativesDirectory { get; }
    
    public string LoggingArgument { get; }
    public string LoggingFile { get; }
    
    public IReadOnlyList<string> Libraries { get; }
}