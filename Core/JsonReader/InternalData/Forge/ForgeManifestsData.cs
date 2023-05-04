using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeManifestsData
{
    [JsonPropertyName("data")]
    public ForgeManifestData[]? Data { get; set; }
}