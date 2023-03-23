using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Assets;

internal class AssetData
{
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }
    
    [JsonPropertyName("size")]
    public int Size { get; set; }
}