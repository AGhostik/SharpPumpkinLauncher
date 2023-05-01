namespace Launcher.Tools;

internal sealed class Features
{
    public Features(bool useCustomResolution)
    {
        UseCustomResolution = useCustomResolution;
    }

    public bool UseCustomResolution { get; }
}