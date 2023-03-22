using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class NativesData
{
    [JsonPropertyName("linux")]
    public string? Linux { get; set; }

    [JsonPropertyName("windows")]
    public string? Windows { get; set; }

    [JsonPropertyName("osx")]
    public string? Osx { get; set; }
}