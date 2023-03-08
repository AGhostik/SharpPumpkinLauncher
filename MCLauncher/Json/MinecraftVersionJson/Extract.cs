using Newtonsoft.Json;

namespace MCLauncher.Json.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Extract
{
    [JsonProperty("exclude")]
    public string[]? Exclude { get; set; }
}