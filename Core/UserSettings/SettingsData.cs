namespace UserSettings;

public sealed class SettingsData
{
    public string? LastProfileName { get; set; }
    public List<ProfileData>? Profiles { get; set; }
    public int LauncherVisibility { get; set; }
    public string? GameDirectory { get; set; }
    public string? DefaultPlayerName { get; set; }
    public bool IsJavaInstalled { get; set; }
}