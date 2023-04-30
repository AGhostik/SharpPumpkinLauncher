using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimeFileDownloadsData
{
    [JsonPropertyName("lzma")]
    public RuntimeDownloadData? Lzma { get; set; }
    
    [JsonPropertyName("raw")]
    public RuntimeDownloadData? Raw { get; set; }
}