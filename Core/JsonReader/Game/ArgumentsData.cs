using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class ArgumentsData
{
    [JsonPropertyName("game")]
    public ArgumentItemData[]? Game { get; set; }

    [JsonPropertyName("jvm")]
    public ArgumentItemData[]? Jvm { get; set; }
}