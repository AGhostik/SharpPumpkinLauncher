using System;
using System.Collections.Generic;

namespace SharpPumpkinLauncher.Main.Settings;

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
        UseJavaArguments = false;
        Arguments = Array.Empty<string>();
    }
    
    public SettingsData(string? defaultPlayerName, string directory, LauncherVisibility launcherVisibility, 
        bool useCustomResolution, int screenHeight, int screenWidth, bool useJavaArguments, 
        IReadOnlyList<string> arguments)
    {
        DefaultPlayerName = defaultPlayerName;
        Directory = directory;
        LauncherVisibility = launcherVisibility;
        UseCustomResolution = useCustomResolution;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
        UseJavaArguments = useJavaArguments;
        Arguments = arguments;
    }

    public string? DefaultPlayerName { get; }
    public string Directory { get; }
    public LauncherVisibility LauncherVisibility { get; }
    public bool UseCustomResolution { get; }
    public int ScreenHeight { get; }
    public int ScreenWidth { get; }
    public bool UseJavaArguments { get; }
    public IReadOnlyList<string> Arguments { get; }
}