using CommunityToolkit.Mvvm.ComponentModel;
using UserSettings;

namespace MCLauncher.LauncherWindow;

public sealed class ProfileViewModel : ObservableObject
{
    public ProfileViewModel()
    {
        //
    }

    public ProfileViewModel(ProfileData profileData)
    {
        Name = profileData.Name;
        PlayerNickname = profileData.PlayerNickname;
        GameDirectory = profileData.GameDirectory;
        JvmArgs = profileData.JvmArgs;
        MinecraftVersion = profileData.MinecraftVersion;
    }
    
    public string? Name { get; set; }
    public string? PlayerNickname { get; set; }
    public string? GameDirectory { get; set; }
    public string? JvmArgs { get; set; }
    public string? MinecraftVersion { get; set; }
}