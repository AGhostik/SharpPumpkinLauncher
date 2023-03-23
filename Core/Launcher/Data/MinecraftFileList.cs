namespace Launcher.Data;

internal sealed class MinecraftFileList
{
    public MinecraftFileList(MinecraftFile client, MinecraftFile server,
        IReadOnlyList<MinecraftLibraryFile> libraryFiles, IReadOnlyList<MinecraftFile> assetFiles)
    {
        Client = client;
        Server = server;
        LibraryFiles = libraryFiles;
        AssetFiles = assetFiles;
    }

    public MinecraftFile Client { get; }
    public MinecraftFile Server { get; }
    public MinecraftFile? Logging { get; set; }
    public IReadOnlyList<MinecraftLibraryFile> LibraryFiles { get; }
    public IReadOnlyList<MinecraftFile> AssetFiles { get; }
}