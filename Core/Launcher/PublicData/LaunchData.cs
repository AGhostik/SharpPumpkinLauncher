namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, Version version, string gameDirectory, bool useCustomResolution,
        int screenHeight, int screenWidth)
    {
        PlayerName = playerName;
        GameDirectory = gameDirectory;
        UseCustomResolution = useCustomResolution;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
        Version = version;
    }

    public string PlayerName { get; }
    public string GameDirectory { get; }
    public Version Version { get; }
    public bool UseCustomResolution { get; }
    public int ScreenHeight { get; }
    public int ScreenWidth { get; }
}