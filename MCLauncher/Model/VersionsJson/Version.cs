using System;
using Newtonsoft.Json;

namespace MCLauncher.Model.VersionsJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Version
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("time")] public DateTime Time { get; set; }

        [JsonProperty("releaseTime")] public DateTime ReleaseTime { get; set; }

        [JsonProperty("type")] public string Type { get; set; }
    }
}
