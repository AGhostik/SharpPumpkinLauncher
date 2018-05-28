using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DownloadInfo
    {
        [JsonProperty("size")] public int Size { get; set; }

        [JsonProperty("sha1")] public string Sha1 { get; set; }

        [JsonProperty("path")] public string Path { get; set; }

        [JsonProperty("url")] public string Url { get; set; }
    }
}
