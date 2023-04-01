using JsonReader.PublicData.Manifest;
using JsonReader.Tools;

namespace JsonReader.PublicData.Game;

public sealed class MinecraftData
{
    public MinecraftData(string id, string type, string assetsVersion, string mainClass, int minimumLauncherVersion,
        DateTime releaseTime, DateTime time, DownloadFile client, DownloadFile server, DownloadFile assetsIndex,
        Logging? loggingData, Arguments arguments, IReadOnlyList<Library> libraries)
    {
        Id = id;
        Type = type;
        MinecraftType = MinecraftTypeConverter.GetMinecraftType(type);
        AssetsVersion = assetsVersion;
        MainClass = mainClass;
        MinimumLauncherVersion = minimumLauncherVersion;
        ReleaseTime = releaseTime;
        Time = time;
        Arguments = arguments;
        Libraries = libraries;
        Client = client;
        Server = server;
        AssetsIndex = assetsIndex;
        LoggingData = loggingData;
    }

    public string Id { get; }
    public string Type { get; }
    public MinecraftType MinecraftType { get; }
    public string AssetsVersion { get; }
    public string MainClass { get; }
    public int MinimumLauncherVersion { get; }
    public DateTime ReleaseTime { get; }
    public DateTime Time { get; }
    public DownloadFile Client { get; }
    public DownloadFile Server { get; }
    public DownloadFile AssetsIndex { get; }
    public Logging? LoggingData { get; }
    public Arguments Arguments { get; }
    public IReadOnlyList<Library> Libraries { get; }
}