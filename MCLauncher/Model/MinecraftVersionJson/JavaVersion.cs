using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class JavaVersion
{
    [JsonProperty("component")]
    public string? Component { get; set; }

    [JsonProperty("majorVersion")]
    public int MajorVersion { get; set; }
}