using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeParentData
{
    [JsonPropertyName("data")]
    public ForgeData? Data { get; set; }
}