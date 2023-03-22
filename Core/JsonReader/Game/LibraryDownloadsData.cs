using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class LibraryDownloadsData
{
    [JsonPropertyName("artifact")]
    public ArtifactData? Artifact { get; set; }

    [JsonPropertyName("classifiers")]
    public ClassifiersData? Classifiers { get; set; }
}