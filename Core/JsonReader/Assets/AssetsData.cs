using System.Text.Json.Serialization;

namespace JsonReader.Assets;

public class AssetsData
{
    [JsonPropertyName("objects")]
    public Dictionary<string, AssetData>? AssetList { get; set; }
}