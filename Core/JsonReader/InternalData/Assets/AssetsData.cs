using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Assets;

internal class AssetsData
{
    [JsonPropertyName("objects")]
    public Dictionary<string, AssetData>? AssetList { get; set; }
}