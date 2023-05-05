using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeArgumentsData
{
    [JsonPropertyName("game")]
    public string[]? Game { get; set; }

    [JsonPropertyName("jvm")]
    public string[]? Jvm { get; set; }
}