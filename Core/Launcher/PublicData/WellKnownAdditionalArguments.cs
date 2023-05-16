namespace Launcher.PublicData;

public static class WellKnownAdditionalArguments
{
    public const string AggressiveHeap = "-XX:+AggressiveHeap";
    public const string AlwaysPreTouch = "-XX:+AlwaysPreTouch";
    public const string DisableExplicitGc = "-XX:+DisableExplicitGC";
    public const string ParallelRefProcEnabled = "-XX:+ParallelRefProcEnabled";
    public const string UseCompressedOops = "-XX:+UseCompressedOops";
    public const string UseG1Gc = "-XX:+UseG1GC";
    public const string UnlockExperimentalVmOptions = "-XX:+UnlockExperimentalVMOptions";
    public const string G1UseAdaptiveIhop = "-XX:+G1UseAdaptiveIHOP";
    public const string UseStringDeduplication = "-XX:+UseStringDeduplication";
    public const string PerfDisableSharedMem = "-XX:+PerfDisableSharedMem";
    
    public const string Xmn = "-Xmn";
    public const string Xmx = "-Xmx";
    public const string Xms = "-Xms";
    
    public const string ParallelGcThreads = "-XX:ParallelGCThreads";
    public const string ConcGcThreads = "-XX:ConcGCThreads";
    public const string MaxGcPauseMillis = "-XX:MaxGCPauseMillis";
    public const string G1HeapRegionSize = "-XX:G1HeapRegionSize";
    public const string G1NewSizePercent = "-XX:G1NewSizePercent";
    public const string G1MaxNewSizePercent = "-XX:G1MaxNewSizePercent";
    public const string G1ReservePercent = "-XX:G1ReservePercent";
    public const string G1MixedGcCountTarget = "-XX:G1MixedGCCountTarget";
    public const string G1MixedGcLiveThresholdPercent = "-XX:G1MixedGCLiveThresholdPercent";
    public const string InitiatingHeapOccupancyPercent = "-XX:InitiatingHeapOccupancyPercent";
}