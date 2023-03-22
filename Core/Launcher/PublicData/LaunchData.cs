namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, string versionId, string gameDirectory, string javaFile)
    {
        PlayerName = playerName;
        VersionId = versionId;
        GameDirectory = gameDirectory;
        JavaFile = javaFile;
    }

    public string PlayerName { get; }
    
    public string VersionId { get; }
    public string GameDirectory { get; }
    public string JavaFile { get; }
}