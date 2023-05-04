using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeManifestData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("gameVersion")]
    public string? GameVersion { get; set; }
    
    [JsonPropertyName("latest")]
    public bool Latest { get; set; }
    
    [JsonPropertyName("recommended")]
    public bool Recommended { get; set; }
    
    [JsonPropertyName("dateModified")]
    public DateTime DateModified { get; set; }
    
    [JsonPropertyName("type")]
    public int Type { get; set; }
}