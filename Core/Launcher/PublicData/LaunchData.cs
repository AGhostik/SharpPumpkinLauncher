namespace Launcher.PublicData;

public sealed class LaunchData
{
    public LaunchData(string playerName, string gameDirectory, Version version)
    {
        PlayerName = playerName;
        GameDirectory = gameDirectory;
        Version = version;
    }

    public string PlayerName { get; }
    public string GameDirectory { get; }
    public Version Version { get; }
}