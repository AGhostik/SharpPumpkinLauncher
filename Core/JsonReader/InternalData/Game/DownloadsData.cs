using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class DownloadsData
{
    [JsonPropertyName("client")]
    public DownloadData? Client { get; set; }

    [JsonPropertyName("server")]
    public DownloadData? Server { get; set; }
}