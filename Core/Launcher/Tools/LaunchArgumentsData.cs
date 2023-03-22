using JsonReader.Game;
using Launcher.Data;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsData
{
    public LaunchArgumentsData(FileManager fileManager, MinecraftVersionData minecraftVersionData,
        MinecraftFileList fileList, MinecraftPaths minecraftPaths, string playerName)
    {
        VersionId = minecraftVersionData.Id ?? "null";
        VersionType = minecraftVersionData.Type ?? "null";
        ClientJar = fileManager.GetFullPath(fileList.Client?.FileName);
        PlayerName = playerName;

        AssetsVersion = minecraftVersionData.Assets ?? "null";

        GameDirectory = fileManager.GetFullPath(minecraftPaths.GameDirectory);
        AssetsDirectory = fileManager.GetFullPath(minecraftPaths.AssetsDirectory);
        LibrariesDirectory = fileManager.GetFullPath(minecraftPaths.LibrariesDirectory);
        NativesDirectory = fileManager.GetFullPath(minecraftPaths.NativesDirectory);

        LoggingArgument = minecraftVersionData.Logging?.Client?.Argument ?? "null";
        LoggingFile = string.IsNullOrEmpty(fileList.Logging?.FileName) ? "null" : $"\"{fileManager.GetFullPath(fileList.Logging?.FileName)}\"";

        var lib = new string[fileList.LibraryFiles.Count];
        for (var i = 0; i < fileList.LibraryFiles.Count; i++)
            lib[i] = fileManager.GetFullPath(fileList.LibraryFiles[i].FileName);

        Libraries = lib;
    }

    public string LauncherName { get; } = "\"mclauncher\"";

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