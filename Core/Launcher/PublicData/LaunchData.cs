namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, Version version, ForgeVersion? forgeVersion, string gameDirectory, 
        bool useCustomResolution, int screenHeight, int screenWidth)
    {
        PlayerName = playerName;
        GameDirectory = gameDirectory.TrimEnd('\\').TrimEnd('/');
        UseCustomResolution = useCustomResolution;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
        Version = version;
        ForgeVersion = forgeVersion;
    }

    public string PlayerName { get; }
    public string GameDirectory { get; }
    public Version Version { get; }
    public ForgeVersion? ForgeVersion { get; }
    public bool UseCustomResolution { get; }
    public int ScreenHeight { get; }
    public int ScreenWidth { get; }
}