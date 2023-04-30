using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimeAvailabilityData
{
    [JsonPropertyName("group")]
    public int Group { get; set; }
    
    [JsonPropertyName("progress")]
    public int Progress { get; set; }
}