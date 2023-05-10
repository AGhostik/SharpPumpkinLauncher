using Avalonia.Media;

namespace MinecraftLauncher.Resources;

public static class SvgIconsData
{
    static SvgIconsData()
    {
        DeleteProfileIcon = PathGeometry.Parse(SvgPaths.DeleteProfileIconPath);
        AddProfileIcon = PathGeometry.Parse(SvgPaths.AddProfileIconPath);
        EditProfileIcon = PathGeometry.Parse(SvgPaths.EditProfileIconPath);
        SettingsIcon = PathGeometry.Parse(SvgPaths.SettingsIconPath);
        PlayIcon = PathGeometry.Parse(SvgPaths.PlayIconPath);
        CloseIcon = PathGeometry.Parse(SvgPaths.CloseIconPath);
        InfoIcon = PathGeometry.Parse(SvgPaths.InfoIconPath);
    }
    
    public static readonly Geometry DeleteProfileIcon;
    public static readonly Geometry AddProfileIcon;
    public static readonly Geometry EditProfileIcon;
    public static readonly Geometry SettingsIcon;
    public static readonly Geometry PlayIcon;
    public static readonly Geometry CloseIcon;
    public static readonly Geometry InfoIcon;
}