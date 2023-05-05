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

    public string Id { get; }
    public IReadOnlyList<string> Tags { get; }

    public override bool Equals(object? obj)
    {
        if (obj is VersionViewModel versionViewModel)
            return Equals(versionViewModel);
        
        return false;
    }

    public bool Equals(VersionViewModel other)
    {
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}