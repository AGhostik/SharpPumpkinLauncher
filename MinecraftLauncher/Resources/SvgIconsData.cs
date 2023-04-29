using Avalonia.Media;

namespace MinecraftLauncher.Resources;

public static class SvgIconsData
{
    static SvgIconsData()
    {
        InternetIcon = PathGeometry.Parse(SvgPaths.InternetIconPath);
        CopyIcon = PathGeometry.Parse(SvgPaths.CopyIconPath);
    }
    
    public static readonly Geometry InternetIcon;
    public static readonly Geometry CopyIcon;
}