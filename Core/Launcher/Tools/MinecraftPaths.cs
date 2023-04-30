namespace Launcher.Tools;

internal sealed class MinecraftPaths
{
    public MinecraftPaths(string gameDirectory, string versionId)
    {
        GameDirectory = gameDirectory;
        AssetsDirectory = $"{gameDirectory}\\assets";
        AssetsIndexesDirectory = $"{AssetsDirectory}\\indexes";
        AssetsObjectsDirectory = $"{AssetsDirectory}\\objects";
        AssetsLegacyDirectory = $"{AssetsDirectory}\\virtual\\legacy";
        LibrariesDirectory = $"{gameDirectory}\\libraries";
        VersionDirectory = $"{gameDirectory}\\versions\\{versionId}";
        NativesDirectory = $"{VersionDirectory}\\natives";
        RuntimeDirectory = $"{gameDirectory}\\runtime";
    }
    
    public string GameDirectory { get; }
    public string AssetsDirectory { get; }
    public string AssetsIndexesDirectory { get; }
    public string AssetsObjectsDirectory { get; }
    public string AssetsLegacyDirectory { get; }
    public string LibrariesDirectory { get; }
    public string VersionDirectory { get; }
    public string NativesDirectory { get; }
    public string RuntimeDirectory { get; }
}