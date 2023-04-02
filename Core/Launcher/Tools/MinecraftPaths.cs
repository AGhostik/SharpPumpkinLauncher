namespace Launcher.Tools;

internal sealed class MinecraftPaths
{
    public MinecraftPaths(string gameDirectory, string versionId)
    {
        GameDirectory = gameDirectory;
        AssetsDirectory = $"{gameDirectory}\\assets";
        AssetsIndexDirectory = $"{AssetsDirectory}\\indexes";
        LegacyAssetsDirectory = $"{AssetsDirectory}\\virtual\\legacy";
        LibrariesDirectory = $"{gameDirectory}\\libraries";
        VersionDirectory = $"{gameDirectory}\\versions\\{versionId}";
        NativesDirectory = $"{VersionDirectory}\\natives";
    }
    
    public string GameDirectory { get; }
    public string AssetsDirectory { get; }
    public string AssetsIndexDirectory { get; }
    public string LegacyAssetsDirectory { get; }
    public string LibrariesDirectory { get; }
    public string VersionDirectory { get; }
    public string NativesDirectory { get; }
}