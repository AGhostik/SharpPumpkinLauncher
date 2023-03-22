namespace Launcher.Data;

internal sealed class MinecraftFileList
{
    public MinecraftFile? Client { get; set; }
    public MinecraftFile? Server { get; set; }
    public MinecraftFile? Logging { get; set; }
    public List<MinecraftLibraryFile> LibraryFiles { get; } = new();
    public List<MinecraftFile> AssetFiles { get; } = new();
}