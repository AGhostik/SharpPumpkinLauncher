namespace UserSettings;

public sealed class ProfileData
{
    public string? Name { get; set; }
    public string? PlayerNickname { get; set; }
    public string? MinecraftVersion { get; set; }
    
    public bool Alpha { get; set; }
    public bool Beta { get; set; }
    public bool Snapshot { get; set; }
    public bool Release { get; set; }
    public bool Custom { get; set; }
}