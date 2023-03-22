using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class ExtractData
{
    [JsonPropertyName("exclude")]
    public string[]? Exclude { get; set; }
}