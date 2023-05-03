using MinecraftLauncher.Main;
using MinecraftLauncher.Main.Profile;

namespace MinecraftLauncher;

public sealed class ServiceProvider
{
    static ServiceProvider()
    {
        SettingsManager = new SettingsManager();
        MinecraftLauncher = new Launcher.MinecraftLauncher();
        
        ProfileModel = new ProfileModel(MinecraftLauncher);
        VersionsLoader = new VersionsLoader(MinecraftLauncher, SettingsManager);
        MainWindowModel = new MainWindowModel(MinecraftLauncher, VersionsLoader, SettingsManager);
    }
    
    public static Launcher.MinecraftLauncher MinecraftLauncher { get; }
    public static VersionsLoader VersionsLoader { get; }
    public static MainWindowModel MainWindowModel { get; }
    public static ProfileModel ProfileModel { get; }
    public static SettingsManager SettingsManager { get; }
}