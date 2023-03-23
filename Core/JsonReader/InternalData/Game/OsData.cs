using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class OsData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    [JsonPropertyName("arch")]
    public string? Architecture { get; set; }
}