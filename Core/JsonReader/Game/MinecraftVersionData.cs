using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class MinecraftVersionData
{
    [JsonPropertyName("arguments")]
    public ArgumentsData? Arguments { get; set; }

    [JsonPropertyName("assetIndex")]
    public AssetIndexData? AssetIndex { get; set; }

    [JsonPropertyName("assets")]
    public string? Assets { get; set; }

    [JsonPropertyName("downloads")]
    public DownloadsData? Downloads { get; set; }

    [JsonPropertyName("javaVersion")]
    public JavaVersionData? JavaVersion { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("libraries")]
    public LibraryData[]? Library { get; set; }
    
    [JsonPropertyName("logging")]
    public Logging? Logging { get; set; }

    [JsonPropertyName("mainClass")]
    public string? MainClass { get; set; }

    [JsonPropertyName("minecraftArguments")]
    public string? MinecraftArguments { get; set; }

    [JsonPropertyName("minimumLauncherVersion")]
    public int MinimumLauncherVersion { get; set; }

    [JsonPropertyName("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}