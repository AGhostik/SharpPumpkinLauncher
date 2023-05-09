namespace Launcher.PublicData;

public sealed class LaunchFeaturesData
{
    public LaunchFeaturesData(bool useCustomResolution, int screenHeight, int screenWidth)
    {
        UseCustomResolution = useCustomResolution;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
    }

    public bool UseCustomResolution { get; }
    public int ScreenHeight { get; }
    public int ScreenWidth { get; }
}