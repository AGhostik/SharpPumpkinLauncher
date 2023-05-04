namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, string versionId, string? forgeVersionId, string gameDirectory, 
        bool useCustomResolution, int screenHeight, int screenWidth)
    {
        PlayerName = playerName;
        GameDirectory = gameDirectory.TrimEnd('\\').TrimEnd('/');
        UseCustomResolution = useCustomResolution;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
        VersionId = versionId;
        ForgeVersionId = forgeVersionId;
    }

    public string PlayerName { get; }
    public string GameDirectory { get; }
    public string VersionId { get; }
    public string? ForgeVersionId { get; }
    public bool UseCustomResolution { get; }
    public int ScreenHeight { get; }
    public int ScreenWidth { get; }
}