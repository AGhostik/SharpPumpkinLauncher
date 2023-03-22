using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class DownloadData
{
    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}