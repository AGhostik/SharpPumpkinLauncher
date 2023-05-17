namespace Launcher.Data;

internal interface IMinecraftFileList
{
    MinecraftFile Client { get; }
    MinecraftFile? Server { get; }
    MinecraftFile? Logging { get; }
    IReadOnlyList<MinecraftLibraryFile> LibraryFiles { get; }
    IReadOnlyList<MinecraftFile> AssetFiles { get; }
    IReadOnlyList<MinecraftFile> JavaRuntimeFiles { get; }
}