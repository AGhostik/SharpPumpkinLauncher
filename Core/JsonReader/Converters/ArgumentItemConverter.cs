using System.Text.Json;
using System.Text.Json.Serialization;
using JsonReader.Game;

namespace JsonReader.Converters;

public class ArgumentItemConverter : JsonConverter<ArgumentItemData>
{
    public override ArgumentItemData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new ArgumentItemData();
        
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                result.Value = new[] { reader.GetString() };
                break;
            case JsonTokenType.StartObject:
            {
                using var doc = JsonDocument.ParseValue(ref reader);
            
                if (doc.RootElement.TryGetProperty("rules", out var rulesElement))
                {
                    var rules = rulesElement.Deserialize<RulesData[]>();
                    result.Rules = rules;
                }
                
                if (doc.RootElement.TryGetProperty("value", out var valueElement))
                {
                    switch (valueElement.ValueKind)
                    {
                        case JsonValueKind.String:
                        {
                            var str = valueElement.GetString();
                            if (!string.IsNullOrEmpty(str))
                                result.Value = new[] { str };
                            break;
                        }
                        case JsonValueKind.Array:
                        {
                            var index = 0;
                            result.Value = new string[valueElement.GetArrayLength()];
                            foreach (var item in valueElement.EnumerateArray())
                            {
                                result.Value[index] = item.GetString();
                                index++;
                            }

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ArgumentItemData value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}