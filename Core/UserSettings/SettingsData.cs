namespace UserSettings;

public sealed class SettingsData
{
    public string? LastProfileName { get; set; }
    public List<ProfileData>? Profiles { get; set; }

    public int LauncherVisibility { get; set; }

    public bool Alpha { get; set; }
    public bool Beta { get; set; }
    public bool Snapshot { get; set; }
    public bool Release { get; set; }
    public bool Custom { get; set; }
}