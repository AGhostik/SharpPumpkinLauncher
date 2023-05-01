namespace UserSettings;

public sealed class SettingsData
{
    public string? LastProfileName { get; set; }
    public List<ProfileData>? Profiles { get; set; }
    public int LauncherVisibility { get; set; }
    public string? GameDirectory { get; set; }
    public string? DefaultPlayerName { get; set; }
    public bool UseCustomResolution { get; set; }
    public int ScreenHeight { get; set; }
    public int ScreenWidth { get; set; }
}