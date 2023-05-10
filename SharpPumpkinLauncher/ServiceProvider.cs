namespace SharpPumpkinLauncher;

public static class ServiceProvider
{
    static ServiceProvider()
    {
        SettingsManager = new SettingsManager();
        MinecraftLauncher = new Launcher.MinecraftLauncher();
        
        VersionsLoader = new VersionsLoader(MinecraftLauncher, SettingsManager);
    }
    
    public static Launcher.MinecraftLauncher MinecraftLauncher { get; }
    public static VersionsLoader VersionsLoader { get; }
    public static SettingsManager SettingsManager { get; }
}