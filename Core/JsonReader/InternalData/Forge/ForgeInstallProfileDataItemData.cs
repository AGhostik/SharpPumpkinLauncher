using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeInstallProfileDataItemData
{
    [JsonPropertyName("client")]
    public string? Client { get; set; }
    
    [JsonPropertyName("server")]
    public string? Server { get; set; }
}