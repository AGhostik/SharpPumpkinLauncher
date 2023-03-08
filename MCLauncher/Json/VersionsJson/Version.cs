using System;
using Newtonsoft.Json;

namespace MCLauncher.Json.VersionsJson;

[JsonObject(MemberSerialization.OptIn)]
public class Version
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("time")]
    public DateTime Time { get; set; }

    [JsonProperty("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    [JsonProperty("sha1")]
    public string? Sha1 { get; set; }

    [JsonProperty("complianceLevel")]
    public int ComplianceLevel { get; set; }
}