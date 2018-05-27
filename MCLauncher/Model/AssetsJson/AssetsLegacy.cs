using Newtonsoft.Json;

namespace MCLauncher.Model.AssetsJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AssetsLegacy
    {
        [JsonProperty("virtual")]
        public bool Virtual { get; set; }

        [JsonProperty("objects")]
        public string[] Objects { get; set; }
    }
}
