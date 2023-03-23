using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class NativesData
{
    [JsonPropertyName("linux")]
    public string? Linux { get; set; }

    [JsonPropertyName("windows")]
    public string? Windows { get; set; }

    [JsonPropertyName("osx")]
    public string? Osx { get; set; }
}