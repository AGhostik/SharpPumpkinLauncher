using JsonReader.PublicData.Game;

namespace JsonReader.PublicData.Forge;

public sealed class ForgeInstall
{
    public ForgeInstall(string profile, string version, string json, string path, string logo, string minecraft,
        ForgeInstallData? data, IReadOnlyList<ForgeInstallProcessor>? processors, IReadOnlyList<Library> libraries)
    {
        Profile = profile;
        Version = version;
        Json = json;
        Path = path;
        Logo = logo;
        Minecraft = minecraft;
        Data = data;
        Processors = processors;
        Libraries = libraries;
    }

    public string Profile { get; }
    
    public string Version { get; }
    
    public string Json { get; }
    
    public string Path { get; }
    
    public string Logo { get; }
    
    public string Minecraft { get; }
    
    public ForgeInstallData? Data { get; }
    
    public IReadOnlyList<ForgeInstallProcessor>? Processors { get;  }
    
    public IReadOnlyList<Library> Libraries { get; }
}