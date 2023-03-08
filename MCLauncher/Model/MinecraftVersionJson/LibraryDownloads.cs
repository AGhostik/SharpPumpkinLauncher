using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson;

[JsonObject(MemberSerialization.OptIn)]
public class LibraryDownloads
{
    [JsonProperty("artifact")]
    public Artifact? Artifact { get; set; }
    
    [JsonProperty("classifiers")]
    public Classifiers? Classifiers { get; set; }
}