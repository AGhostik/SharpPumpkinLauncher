using System;
using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class Library
{
    [JsonProperty("downloads")]
    public LibraryDownloads? Downloads { get; set; }
    
    [JsonProperty("extract")]
    public Extract? Extract { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("natives")]
    public Natives? Natives { get; set; }

    [JsonProperty("rules")]
    public Rules[]? Rules { get; set; }
    
    [JsonProperty("mainClass")]
    public string? MainClass { get; set; }
        
    [JsonProperty("minecraftArguments")]
    public string? MinecraftArguments { get; set; }
        
    [JsonProperty("minimumLauncherVersion")]
    public string? MinimumLauncherVersion { get; set; }
        
    [JsonProperty("releaseTime")]
    public DateTime? ReleaseTime { get; set; }

    [JsonProperty("time")]
    public DateTime? Time { get; set; }
        
    [JsonProperty("type")]
    public string? Type { get; set; }
}