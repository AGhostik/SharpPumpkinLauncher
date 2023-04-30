using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class RuntimesData
{
    [JsonPropertyName("java-runtime-alpha")]
    public RuntimeData[]? JavaRuntimeAlpha { get; set; }
    
    [JsonPropertyName("java-runtime-beta")]
    public RuntimeData[]? JavaRuntimeBeta { get; set; }
    
    [JsonPropertyName("java-runtime-gamma")]
    public RuntimeData[]? JavaRuntimeGamma { get; set; }
    
    [JsonPropertyName("jre-legacy")]
    public RuntimeData[]? JreLegacy { get; set; }
    
    [JsonPropertyName("minecraft-java-exe")]
    public RuntimeData[]? MinecraftJavaExe { get; set; }
}