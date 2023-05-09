using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeInstallProfileProcessorData
{
    [JsonPropertyName("jar")]
    public string? Jar { get; set; }
    
    [JsonPropertyName("classpath")]
    public string[]? Classpath { get; set; }
    
    [JsonPropertyName("args")]
    public string[]? Args { get; set; }
    
    [JsonPropertyName("sides")]
    public string[]? Sides { get; set; }
    
    [JsonPropertyName("outputs")]
    public Dictionary<string, string>? Outputs { get; set; }
}