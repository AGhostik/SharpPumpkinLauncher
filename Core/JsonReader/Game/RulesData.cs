using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class RulesData
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("os")]
    public OsData? Os { get; set; }
}