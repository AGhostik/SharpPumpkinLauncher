using System.Text.Json.Serialization;
using JsonReader.Converters;

namespace JsonReader.Game;

[JsonConverter(typeof(ArgumentItemConverter))]
public class ArgumentItemData
{
    public string?[]? Value { get; set; }
    public RulesData[]? Rules { get; set; }
}