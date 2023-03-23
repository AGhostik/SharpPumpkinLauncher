using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class ArtifactData
{
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}