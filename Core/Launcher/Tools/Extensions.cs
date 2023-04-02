using JsonReader.PublicData.Game;

namespace Launcher.Tools;

internal static class Extensions
{
    public static bool IsLegacyAssets(this MinecraftData minecraftData)
    {
        return minecraftData.AssetsVersion is "legacy" or "pre-1.6";
    }
}