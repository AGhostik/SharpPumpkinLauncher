using System.Text.Json.Serialization;

namespace JsonReader.Assets;

public class AssetData
{
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }
    
    [JsonPropertyName("size")]
    public int Size { get; set; }
}