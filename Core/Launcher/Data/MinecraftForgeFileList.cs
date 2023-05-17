namespace Launcher.Data;

internal class MinecraftForgeFileList : IMinecraftFileList
{
    public MinecraftForgeFileList(MinecraftFileList minecraftFileList, 
        IReadOnlyList<MinecraftLibraryFile> forgeLibraryFiles, 
        string javaFilePath, MinecraftFile? srg, MinecraftFile? extra, MinecraftFile? forgeClient, 
        IReadOnlyList<MinecraftLibraryFile> profileLibraryFiles)
    {
        var libraries = new List<MinecraftLibraryFile>(minecraftFileList.LibraryFiles.Count + forgeLibraryFiles.Count);
        libraries.AddRange(forgeLibraryFiles);
        libraries.AddRange(minecraftFileList.LibraryFiles);

        Client = minecraftFileList.Client;
        Server = minecraftFileList.Server;
        Logging = minecraftFileList.Logging;
        LibraryFiles = libraries;
        AssetFiles = minecraftFileList.AssetFiles;
        JavaRuntimeFiles = minecraftFileList.JavaRuntimeFiles;

        JavaFilePath = javaFilePath;
        Srg = srg;
        Extra = extra;
        ForgeClient = forgeClient;
        ProfileLibraryFiles = profileLibraryFiles;
    }

    public MinecraftFile Client { get; }
    public MinecraftFile? Server { get; }
    public MinecraftFile? Logging { get; }
    public IReadOnlyList<MinecraftLibraryFile> LibraryFiles { get; }
    public IReadOnlyList<MinecraftFile> AssetFiles { get; }
    public IReadOnlyList<MinecraftFile> JavaRuntimeFiles { get; }
    
    public string JavaFilePath { get; }
    public MinecraftFile? Srg { get; }
    public MinecraftFile? Extra { get; }
    public MinecraftFile? ForgeClient { get; }
    public IReadOnlyList<MinecraftLibraryFile> ProfileLibraryFiles { get; }
}