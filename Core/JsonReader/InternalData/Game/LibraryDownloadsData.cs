using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class LibraryDownloadsData
{
    [JsonPropertyName("artifact")]
    public ArtifactData? Artifact { get; set; }

    [JsonPropertyName("classifiers")]
    public ClassifiersData? Classifiers { get; set; }
}