using Newtonsoft.Json;

namespace MCLauncher.Model.VersionsJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Latest
    {
        [JsonProperty("snapshot")] public string Snapshoot { get; set; }

        [JsonProperty("release")] public string Release { get; set; }
    }
}
