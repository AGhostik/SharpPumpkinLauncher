namespace Launcher.Data;

internal sealed class MinecraftLaunchFiles
{
    public MinecraftLaunchFiles(string client, string logging, string java, IReadOnlyList<string> libraryFiles)
    {
        Client = client;
        LibraryFiles = libraryFiles;
        Java = java;
        Logging = logging;
    }

    public string Client { get; }
    public string Logging { get; }
    public string Java { get; }
    public IReadOnlyList<string> LibraryFiles { get; }
}