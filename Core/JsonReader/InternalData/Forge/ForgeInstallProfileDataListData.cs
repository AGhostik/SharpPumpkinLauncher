using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeInstallProfileDataListData
{
    [JsonPropertyName("MAPPINGS")]
    public ForgeInstallProfileDataItemData? Mappings { get; set; }
    
    [JsonPropertyName("MOJMAPS")]
    public ForgeInstallProfileDataItemData? Mojmaps { get; set; }
    
    [JsonPropertyName("MERGED_MAPPINGS")]
    public ForgeInstallProfileDataItemData? MergedMappings { get; set; }
    
    [JsonPropertyName("BINPATCH")]
    public ForgeInstallProfileDataItemData? Binpatch { get; set; }
    
    [JsonPropertyName("MC_UNPACKED")]
    public ForgeInstallProfileDataItemData? McUnpacked { get; set; }
    
    [JsonPropertyName("MC_SLIM")]
    public ForgeInstallProfileDataItemData? McSlim { get; set; }
    
    [JsonPropertyName("MC_SLIM_SHA")]
    public ForgeInstallProfileDataItemData? McSlimSha { get; set; }
    
    [JsonPropertyName("MC_EXTRA")]
    public ForgeInstallProfileDataItemData? McExtra { get; set; }
    
    [JsonPropertyName("MC_EXTRA_SHA")]
    public ForgeInstallProfileDataItemData? McExtraSha { get; set; }
    
    [JsonPropertyName("MC_SRG")]
    public ForgeInstallProfileDataItemData? McSrg { get; set; }
    
    [JsonPropertyName("PATCHED")]
    public ForgeInstallProfileDataItemData? Patched { get; set; }
    
    [JsonPropertyName("_PATCHED_SHA")]
    public ForgeInstallProfileDataItemData? PathedSha { get; set; }
    
    [JsonPropertyName("MCP_VERSION")]
    public ForgeInstallProfileDataItemData? MpcVersion { get; set; }
    
    [JsonPropertyName("SIDE")]
    public ForgeInstallProfileDataItemData? Side { get; set; }
}