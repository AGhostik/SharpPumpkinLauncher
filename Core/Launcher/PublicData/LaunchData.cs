namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, string versionId, string? forgeVersionId, string gameDirectory, 
        LaunchFeaturesData featuresData, IReadOnlyList<string> arguments)
    {
        PlayerName = playerName;
        GameDirectory = gameDirectory.TrimEnd('\\').TrimEnd('/');
        VersionId = versionId;
        ForgeVersionId = forgeVersionId;
        FeaturesData = featuresData;
        Arguments = arguments;
    }

    public string PlayerName { get; }
    public string GameDirectory { get; }
    public string VersionId { get; }
    public string? ForgeVersionId { get; }
    public LaunchFeaturesData FeaturesData { get; }
    public IReadOnlyList<string> Arguments { get; }
}