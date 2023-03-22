using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class LoggingClient
{
    [JsonPropertyName("argument")]
    public string? Argument { get; set; }
    
    [JsonPropertyName("file")]
    public LoggingFile? File { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}