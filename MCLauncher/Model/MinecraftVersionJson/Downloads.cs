using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Downloads
{
    [JsonProperty("client")]
    public DownloadInfo? Client { get; set; }
        
    [JsonProperty("server")]
    public DownloadInfo? Server { get; set; }
}