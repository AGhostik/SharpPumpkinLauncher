using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimeVersionData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("released")]
    public DateTime Released { get; set; }
}