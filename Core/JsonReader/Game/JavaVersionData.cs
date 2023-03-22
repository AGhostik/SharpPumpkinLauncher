using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class JavaVersionData
{
    [JsonPropertyName("component")]
    public string? Component { get; set; }

    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; set; }
}