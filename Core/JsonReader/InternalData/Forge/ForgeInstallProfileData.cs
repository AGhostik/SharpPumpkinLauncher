using System.Text.Json.Serialization;
using JsonReader.InternalData.Game;

namespace JsonReader.InternalData.Forge;

internal class ForgeInstallProfileData
{
    [JsonPropertyName("profile")]
    public string? Profile { get; set; }
    
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    [JsonPropertyName("json")]
    public string? Json { get; set; }
    
    [JsonPropertyName("path")]
    public string? Path { get; set; }
    
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }
    
    [JsonPropertyName("minecraft")]
    public string? Minecraft { get; set; }
    
    [JsonPropertyName("data")]
    public ForgeInstallProfileDataListData? Data { get; set; }
    
    [JsonPropertyName("processors")]
    public ForgeInstallProfileProcessorData[]? Processors { get; set; }
    
    [JsonPropertyName("libraries")]
    public LibraryData[]? Libraries { get; set; }
}
