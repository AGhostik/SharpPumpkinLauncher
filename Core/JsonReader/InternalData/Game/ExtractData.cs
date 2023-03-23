using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class ExtractData
{
    [JsonPropertyName("exclude")]
    public string[]? Exclude { get; set; }
}