using System.Threading.Tasks;
using Launcher.PublicData;

namespace MinecraftLauncher.Main.Profile;

public sealed class ProfileModel
{
    private readonly Launcher.MinecraftLauncher _minecraftLauncher;

    public ProfileModel(Launcher.MinecraftLauncher minecraftLauncher)
    {
        _minecraftLauncher = minecraftLauncher;
    }

    public async Task<Versions> RequestForgeVersions(string versionId)
    {
        return await _minecraftLauncher.GetOnlineForgeVersions(versionId);
    }

    public void SaveProfile(ProfileViewModel profileViewModel)
    {
        SettingsManager.SaveProfile(profileViewModel);
    }

    public void DeleteProfile(ProfileViewModel profileViewModel)
    {
        SettingsManager.DeleteProfile(profileViewModel);
    }

    public void ReplaceProfile(string originalProfileName, ProfileViewModel profileViewModel)
    {
        SettingsManager.ReplaceProfile(originalProfileName, profileViewModel);
    }
}