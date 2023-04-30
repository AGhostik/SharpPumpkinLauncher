using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimeFileData
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("executable")]
    public bool? Executable { get; set; }
    
    [JsonPropertyName("downloads")]
    public RuntimeFileDownloadsData? Downloads { get; set; }
}