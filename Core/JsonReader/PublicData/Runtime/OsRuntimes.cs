namespace JsonReader.PublicData.Runtime;

public sealed class OsRuntimes
{
    public OsRuntimes(OsRuntime linux, OsRuntime linuxI386, OsRuntime macOs, OsRuntime macOsArm64, OsRuntime windowsArm64,
        OsRuntime windows64, OsRuntime windows86)
    {
        Linux = linux;
        LinuxI386 = linuxI386;
        MacOs = macOs;
        MacOsArm64 = macOsArm64;
        WindowsArm64 = windowsArm64;
        Windows64 = windows64;
        Windows86 = windows86;
    }

    public OsRuntime Linux { get; }
    
    public OsRuntime LinuxI386 { get; }
    
    public OsRuntime MacOs { get; }
    
    public OsRuntime MacOsArm64 { get; }
    
    public OsRuntime WindowsArm64 { get; }
    
    public OsRuntime Windows64 { get; }
    
    public OsRuntime Windows86 { get; }
}