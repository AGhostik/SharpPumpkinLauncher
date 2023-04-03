using JsonReader.PublicData.Manifest;
using Launcher.PublicData;

namespace Launcher.Tools;

internal static class MinecraftTypeConverter
{
    /// <exception cref="ArgumentOutOfRangeException">Unknowm argument value</exception>
    public static VersionType GetVersionType(MinecraftType version)
    {
        return version switch
        {
            MinecraftType.Release => VersionType.Release,
            MinecraftType.Snapshot => VersionType.Snapshot,
            MinecraftType.Beta => VersionType.Beta,
            MinecraftType.Alpha => VersionType.Alpha,
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, null)
        };
    }
}