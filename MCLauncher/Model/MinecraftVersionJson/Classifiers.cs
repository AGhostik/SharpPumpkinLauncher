using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Classifiers
    {
        [JsonProperty("natives-linux")] public DownloadInfo NativesLinux { get; set; }

        [JsonProperty("natives-osx")] public DownloadInfo NativesOsx { get; set; }

        [JsonProperty("natives-windows")] public DownloadInfo NativesWindows { get; set; }
    }
}
