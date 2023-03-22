using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class LibraryData
{
    [JsonPropertyName("downloads")]
    public LibraryDownloadsData? Downloads { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("natives")]
    public NativesData? Natives { get; set; }

    [JsonPropertyName("rules")]
    public RulesData[]? Rules { get; set; }
}