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
        UseCustomResolution = false;
        ScreenHeight = 0;
        ScreenWidth = 0;
    }
    
    public SettingsData(string? defaultPlayerName, string directory, LauncherVisibility launcherVisibility, 
        bool useCustomResolution, int screenHeight, int screenWidth)
    {
        DefaultPlayerName = defaultPlayerName;
        Directory = directory;
        LauncherVisibility = launcherVisibility;
        UseCustomResolution = useCustomResolution;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
    }

    public string? DefaultPlayerName { get; }
    public string Directory { get; }
    public LauncherVisibility LauncherVisibility { get; }
    public bool UseCustomResolution { get; }
    public int ScreenHeight { get; }
    public int ScreenWidth { get; }
}