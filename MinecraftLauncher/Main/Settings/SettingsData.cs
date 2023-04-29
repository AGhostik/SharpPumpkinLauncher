using System;

namespace MinecraftLauncher.Main.Settings;

public sealed class SettingsData
{
    public SettingsData()
    {
        var random = new Random();
        DefaultPlayerName = $"Steve{random.Next(0, 99999):D5}";
        Directory = "Minecraft/";
        LauncherVisibility = LauncherVisibility.KeepOpen;
    }
    
    public SettingsData(string? defaultPlayerName, string directory, LauncherVisibility launcherVisibility)
    {
        DefaultPlayerName = defaultPlayerName;
        Directory = directory;
        LauncherVisibility = launcherVisibility;
    }

    public string? DefaultPlayerName { get; }
    public string Directory { get; }
    public LauncherVisibility LauncherVisibility { get; }
}