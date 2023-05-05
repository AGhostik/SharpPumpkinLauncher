using System.Text.Json.Serialization;
using JsonReader.InternalData.Game;

namespace JsonReader.InternalData.Forge;

internal class ForgeVersionData
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    
    [JsonPropertyName("releaseTime")]
    public DateTime ReleaseTime { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("minecraftArguments")]
    public string? MinecraftArguments { get; set; }
    
    [JsonPropertyName("arguments")]
    public ForgeArgumentsData? Arguments { get; set; }
    
    [JsonPropertyName("minimumLauncherVersion")]
    public int MinimumLauncherVersion { get; set; }
    
    [JsonPropertyName("inheritsFrom")]
    public string? InheritsFrom { get; set; }
    
    [JsonPropertyName("jar")]
    public string? Jar { get; set; }
    
    [JsonPropertyName("libraries")]
    public LibraryData[]? Libraries { get; set; }
    
    [JsonPropertyName("mainClass")]
    public string? MainClass { get; set; }
}