using System.Text.Json.Serialization;

namespace JsonReader.Game;

public class ClassifiersData
{
    // [JsonProperty("natives-linux")]
    // public Artifact? NativesLinux { get; set; }
    //
    // [JsonProperty("natives-osx")]
    // public Artifact? NativesOsx { get; set; }

    [JsonPropertyName("natives-windows")]
    public ArtifactData? NativesWindows { get; set; }
}