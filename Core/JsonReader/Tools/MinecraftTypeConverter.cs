using JsonReader.PublicData.Manifest;

namespace JsonReader.Tools;

internal static class MinecraftTypeConverter
{
    private const string Alpha = "old_alpha";
    private const string Beta = "old_beta";
    private const string Release = "release";
    private const string Snapshot = "snapshot";

    /// <exception cref="ArgumentOutOfRangeException">Unknown argument value</exception>
    public static MinecraftType GetMinecraftType(string type)
    {
        return type switch
        {
            Release => MinecraftType.Release,
            Snapshot => MinecraftType.Snapshot,
            Beta => MinecraftType.Beta,
            Alpha => MinecraftType.Alpha,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}