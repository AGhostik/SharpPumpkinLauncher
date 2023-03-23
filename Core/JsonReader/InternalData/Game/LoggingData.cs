using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class LoggingData
{
    [JsonPropertyName("client")]
    public LoggingClientData? Client { get; set; }
}