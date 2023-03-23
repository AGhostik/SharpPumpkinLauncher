using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Game;

internal class ClassifiersData
{
    [JsonPropertyName("natives-linux")]
    public ArtifactData? NativesLinux { get; set; }
    
    [JsonPropertyName("natives-osx")]
    public ArtifactData? NativesOsx { get; set; }

    [JsonPropertyName("natives-windows")]
    public ArtifactData? NativesWindows { get; set; }
}