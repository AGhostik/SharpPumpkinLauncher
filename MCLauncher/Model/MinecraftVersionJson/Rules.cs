using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Rules
{
    [JsonProperty("action")]
    public string? Action { get; set; }

    [JsonProperty("os")]
    public Os? Os { get; set; }
}