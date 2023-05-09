namespace Launcher.PublicData;

public struct ForgeInstallProfileProgress
{
    public ForgeInstallProfileProgress(int currentProcessor, int totalProcessor)
    {
        CurrentProcessor = currentProcessor;
        TotalProcessor = totalProcessor;
    }

    public int CurrentProcessor { get; }
    public int TotalProcessor { get; }
}