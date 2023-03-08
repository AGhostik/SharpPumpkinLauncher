using Newtonsoft.Json;

namespace MCLauncher.Json.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Classifiers
{
    // [JsonProperty("natives-linux")]
    // public Artifact? NativesLinux { get; set; }
    //
    // [JsonProperty("natives-osx")]
    // public Artifact? NativesOsx { get; set; }
    
    [JsonProperty("natives-windows")]
    public Artifact? NativesWindows { get; set; }
}