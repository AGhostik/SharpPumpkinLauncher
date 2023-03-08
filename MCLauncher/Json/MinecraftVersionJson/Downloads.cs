using Newtonsoft.Json;

namespace MCLauncher.Json.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Downloads
{
    [JsonProperty("client")]
    public DownloadInfo? Client { get; set; }
        
    [JsonProperty("server")]
    public DownloadInfo? Server { get; set; }
}