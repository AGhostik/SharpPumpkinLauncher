using System.Text.Json.Serialization;

namespace JsonReader.Manifest;

internal class LatestData
{
    [JsonPropertyName("snapshot")]
    public string? Snapshoot { get; set; }

    [JsonPropertyName("release")]
    public string? Release { get; set; }
}