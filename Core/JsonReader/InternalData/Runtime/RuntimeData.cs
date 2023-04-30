using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimeData
{
    [JsonPropertyName("availability")]
    public RuntimeAvailabilityData? Availability { get; set; }
    
    [JsonPropertyName("manifest")]
    public RuntimeManifestData? Manifest { get; set; }
    
    [JsonPropertyName("version")]
    public RuntimeVersionData? Version { get; set; }
}