using System.Text.Json.Serialization;
using JsonReader.Converters;

namespace JsonReader.InternalData.Game;

[JsonConverter(typeof(ArgumentItemConverter))]
internal class ArgumentItemData
{
    public string?[]? Value { get; set; }
    public RulesData[]? Rules { get; set; }
}