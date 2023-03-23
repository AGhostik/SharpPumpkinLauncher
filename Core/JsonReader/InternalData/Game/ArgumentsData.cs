using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class ArgumentsData
{
    [JsonPropertyName("game")]
    public ArgumentItemData[]? Game { get; set; }

    [JsonPropertyName("jvm")]
    public ArgumentItemData[]? Jvm { get; set; }
}