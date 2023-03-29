namespace Launcher.Data;

internal sealed class MinecraftFileList
{
    public MinecraftFileList(MinecraftFile client, MinecraftFile server, MinecraftFile assetsIndex,
        IReadOnlyList<MinecraftLibraryFile> libraryFiles, IReadOnlyList<MinecraftFile> assetFiles)
    {
        Client = client;
        Server = server;
        AssetsIndex = assetsIndex;
        LibraryFiles = libraryFiles;
        AssetFiles = assetFiles;
    }

    public MinecraftFile Client { get; }
    public MinecraftFile Server { get; }
    public MinecraftFile AssetsIndex { get; }
    public MinecraftFile? Logging { get; set; }
    public IReadOnlyList<MinecraftLibraryFile> LibraryFiles { get; }
    public IReadOnlyList<MinecraftFile> AssetFiles { get; }
}