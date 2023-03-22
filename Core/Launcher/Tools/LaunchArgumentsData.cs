using Launcher.Data;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsData
{
    public LaunchArgumentsData(FileManager fileManager,
        string versionId, string versionType, string clientJar, string playerName, string? loggingArgument,
        string? loggingFile, MinecraftPaths minecraftPaths, IReadOnlyList<MinecraftLibraryFile> libraries)
    {
        VersionId = versionId;
        VersionType = versionType;
        ClientJar = fileManager.GetFullPath(clientJar);
        PlayerName = playerName;

        GameDirectory = $"\"{fileManager.GetFullPath(minecraftPaths.GameDirectory)}\"";
        AssetsDirectory = $"\"{fileManager.GetFullPath(minecraftPaths.AssetsDirectory)}\"";
        LibrariesDirectory = $"\"{fileManager.GetFullPath(minecraftPaths.LibrariesDirectory)}\"";
        NativesDirectory = $"\"{fileManager.GetFullPath(minecraftPaths.NativesDirectory)}\"";

        LoggingArgument = string.IsNullOrEmpty(loggingArgument) ? "null" : loggingArgument;
        LoggingFile = string.IsNullOrEmpty(loggingFile) ? "null" : $"\"{fileManager.GetFullPath(loggingFile)}\"";

        var lib = new string[libraries.Count];
        for (var i = 0; i < libraries.Count; i++)
        {
            lib[i] = fileManager.GetFullPath(libraries[i].FileName);
        }

        Libraries = lib;
    }

    public string VersionId { get; }
    public string VersionType { get; }
    public string ClientJar { get; }
    
    public string PlayerName { get; }
        
    public string GameDirectory { get; }
    public string AssetsDirectory { get; }
    public string LibrariesDirectory { get; }
    public string NativesDirectory { get; }
    
    public string LoggingArgument { get; }
    public string LoggingFile { get; }
    
    public IReadOnlyList<string> Libraries { get; }
}