using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimeFilesData
{
    [JsonPropertyName("files")]
    public Dictionary<string, RuntimeFileData>? Files { get; set; }
}