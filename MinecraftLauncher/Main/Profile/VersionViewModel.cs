using System;
using System.Collections.Generic;
using Launcher.PublicData;
using MinecraftLauncher.Properties;
using ReactiveUI;
using Version = Launcher.PublicData.Version;

namespace MinecraftLauncher.Main.Profile;

public class VersionViewModel : ReactiveObject
{
    public VersionViewModel(string id, IReadOnlyList<string>? tags)
    {
        Id = id;
        Tags = tags ?? Array.Empty<string>();
    }

    public VersionViewModel(Version version)
    {
        Id = version.Id;
        Tags = new[] { version.Type.ToString() };
    }
    
    public VersionViewModel(ForgeVersion version)
    {
        Id = version.Id;
        
        var tags = new List<string>(3) { Localization.Forge };
        if (version.IsLatest)
            tags.Add(Localization.Latest);
        if (version.IsRecommended)
            tags.Add(Localization.Recommended);
        Tags = tags;
    }

    public string Id { get; set; }
    public IReadOnlyList<string> Tags { get; }
}