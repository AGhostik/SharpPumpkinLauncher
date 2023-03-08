using Newtonsoft.Json;

namespace MCLauncher.Json.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Natives
{
    [JsonProperty("linux")]
    public string? Linux { get; set; }

    [JsonProperty("windows")]
    public string? Windows { get; set; }

    [JsonProperty("osx")]
    public string? Osx { get; set; }
}