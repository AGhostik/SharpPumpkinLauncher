using Launcher;

namespace SharpPumpkinLauncher;

public static class ServiceProvider
{
    static ServiceProvider()
    {
        SettingsManager = new SettingsManager();
        MinecraftLauncher = new MinecraftLauncher();
        
        VersionsLoader = new VersionsLoader(MinecraftLauncher, SettingsManager);
    }
    
    public static MinecraftLauncher MinecraftLauncher { get; }
    public static VersionsLoader VersionsLoader { get; }
    public static SettingsManager SettingsManager { get; }
}