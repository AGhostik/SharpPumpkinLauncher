using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class OsData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    [JsonPropertyName("arch")]
    public string? Architecture { get; set; }
}