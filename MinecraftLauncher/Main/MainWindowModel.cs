using System.Threading.Tasks;
using Launcher.PublicData;
using MinecraftLauncher.Main.Profile;

namespace MinecraftLauncher.Main;
using Launcher;

public sealed class MainWindowModel
{
    private readonly MinecraftLauncher _minecraftLauncher;

    public MainWindowModel(MinecraftLauncher minecraftLauncher)
    {
        _minecraftLauncher = minecraftLauncher;
    }

    public async Task StartGame(ProfileViewModel profileViewModel)
    {
        var launchData = new LaunchData(
            profileViewModel.PlayerName,
            profileViewModel.SelectedVersion.Id,
            profileViewModel.Directory,
            "java");
        await _minecraftLauncher.LaunchMinecraft(launchData);
    }
}