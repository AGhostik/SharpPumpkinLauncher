using System.Text.Json.Serialization;

namespace JsonReader.Manifest;

public class ManifestData
{
    [JsonPropertyName("latest")]
    public LatestData? Latest { get; set; }

    [JsonPropertyName("versions")]
    public VersionData[]? Versions { get; set; }
}