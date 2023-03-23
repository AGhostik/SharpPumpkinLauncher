using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class LibraryData
{
    [JsonPropertyName("downloads")]
    public LibraryDownloadsData? Downloads { get; set; }
    
    [JsonPropertyName("extract")]
    public ExtractData? Extract { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("natives")]
    public NativesData? Natives { get; set; }

    [JsonPropertyName("rules")]
    public RulesData[]? Rules { get; set; }
}