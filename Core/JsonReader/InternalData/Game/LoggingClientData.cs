using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class LoggingClientData
{
    [JsonPropertyName("argument")]
    public string? Argument { get; set; }
    
    [JsonPropertyName("file")]
    public LoggingFileData? File { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}