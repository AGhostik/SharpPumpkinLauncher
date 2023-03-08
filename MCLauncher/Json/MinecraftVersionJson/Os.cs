using Newtonsoft.Json;

namespace MCLauncher.Json.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Os
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("version")]
    public string? Version { get; set; }
}