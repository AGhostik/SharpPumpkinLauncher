namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, string versionId, string gameDirectory)
    {
        PlayerName = playerName;
        VersionId = versionId;
        GameDirectory = gameDirectory;
    }

    public string PlayerName { get; }
    
    public string VersionId { get; }
    public string GameDirectory { get; }
}