using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class RulesData
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }
    
    [JsonPropertyName("features")]
    public Dictionary<string, bool>? Features { get; set; }

    [JsonPropertyName("os")]
    public OsData? Os { get; set; }
}