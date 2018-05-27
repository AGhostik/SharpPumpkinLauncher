﻿using Newtonsoft.Json;

namespace MCLauncher.Model.MinecraftVersionJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Libraries
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("natives")] public Natives Natives { get; set; }

        [JsonProperty("extract")] public Extract Extract { get; set; }

        [JsonProperty("rules")] public Rules[] Rules { get; set; }
    }
}