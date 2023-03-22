﻿namespace Launcher.Tools;

internal sealed class MinecraftPaths
{
    public MinecraftPaths(string gameDirectory, string versionId)
    {
        GameDirectory = gameDirectory;
        AssetsDirectory = $"{gameDirectory}\\assets";
        LibrariesDirectory = $"{gameDirectory}\\libraries";
        VersionDirectory = $"{gameDirectory}\\versions\\{versionId}";
        NativesDirectory = $"{VersionDirectory}\\natives";

        TemporaryDirectory = $"{gameDirectory}\\temp";
    }
    
    public string GameDirectory { get; }
    public string AssetsDirectory { get; }
    public string LibrariesDirectory { get; }
    public string VersionDirectory { get; }
    public string NativesDirectory { get; }
    
    public string TemporaryDirectory { get; }
}