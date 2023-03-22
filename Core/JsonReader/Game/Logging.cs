using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class Logging
{
    [JsonPropertyName("client")]
    public LoggingClient? Client { get; set; }
}