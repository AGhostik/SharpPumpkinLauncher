using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class JavaVersionData
{
    [JsonPropertyName("component")]
    public string? Component { get; set; }

    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; set; }
}