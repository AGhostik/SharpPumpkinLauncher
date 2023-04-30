using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Runtime;

internal class AllRuntimesData
{
    [JsonPropertyName("linux")]
    public RuntimesData? Linux { get; set; }
    
    [JsonPropertyName("linux-i386")]
    public RuntimesData? LinuxI386 { get; set; }
    
    [JsonPropertyName("mac-os")]
    public RuntimesData? MacOs { get; set; }
    
    [JsonPropertyName("mac-os-arm64")]
    public RuntimesData? MacOsArm64 { get; set; }
    
    [JsonPropertyName("windows-arm64")]
    public RuntimesData? WindowsArm64 { get; set; }
    
    [JsonPropertyName("windows-x64")]
    public RuntimesData? Windows64 { get; set; }
    
    [JsonPropertyName("windows-x86")]
    public RuntimesData? Windows86 { get; set; }
}