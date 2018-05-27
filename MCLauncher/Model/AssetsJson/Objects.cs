using Newtonsoft.Json;

namespace MCLauncher.Model.AssetsJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Objects
    {
        [JsonProperty("hash")] public string Hash { get; set; }

        [JsonProperty("size")] public string Size { get; set; }
    }
}