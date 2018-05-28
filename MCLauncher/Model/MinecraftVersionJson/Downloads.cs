using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Downloads
    {
        [JsonProperty("classifiers")] public Classifiers Classifiers { get; set; }

        [JsonProperty("artifact")] public DownloadInfo Artifact { get; set; }
    }
}
