using Newtonsoft.Json;

namespace MCLauncher.Model.AssetsJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Assets
    {
        [JsonProperty("objects")] public Objects[] Objects { get; set; }
    }
}