namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, string versionId, string? forgeVersionId, string gameDirectory, 
        LaunchFeaturesData featuresData)
    {
        PlayerName = playerName;
        GameDirectory = gameDirectory.TrimEnd('\\').TrimEnd('/');
        VersionId = versionId;
        ForgeVersionId = forgeVersionId;
        FeaturesData = featuresData;
    }

    public string PlayerName { get; }
    public string GameDirectory { get; }
    public string VersionId { get; }
    public string? ForgeVersionId { get; }
    public LaunchFeaturesData FeaturesData { get; }
}