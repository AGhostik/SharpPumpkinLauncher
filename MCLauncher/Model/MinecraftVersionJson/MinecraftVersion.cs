using System;
using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MinecraftVersion
    {
        [JsonProperty("assetIndex")] public AssetIndex AssetIndex { get; set; }

        [JsonProperty("assets")] public string Assets { get; set; }

        //[JsonProperty("downloads")] public string Downloads { get; set; }

        [JsonProperty("id")] public string Id { get; set; }
        
        [JsonProperty("libraries")] public Libraries[] Libraries { get; set; }

        //[JsonProperty("logging")] public string Logging { get; set; }
        
        [JsonProperty("mainClass")] public string MainClass { get; set; }
        
        [JsonProperty("minecraftArguments")] public string MinecraftArguments { get; set; }
        
        [JsonProperty("minimumLauncherVersion")] public string MinimumLauncherVersion { get; set; }
        
        [JsonProperty("releaseTime")] public DateTime ReleaseTime { get; set; }

        [JsonProperty("time")] public DateTime Time { get; set; }
        
        [JsonProperty("type")] public string Type { get; set; }
    }
}
