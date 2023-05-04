using System.Text.Json.Serialization;

namespace JsonReader.InternalData.Forge;

internal class ForgeData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("gameVersionId")]
    public int GameVersionId { get; set; }
    
    [JsonPropertyName("minecraftGameVersionId")]
    public int MinecraftGameVersionId { get; set; }
    
    [JsonPropertyName("forgeVersion")]
    public string? ForgeVersion { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("type")]
    public int Type { get; set; }
    
    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }
    
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
    
    [JsonPropertyName("installMethod")]
    public int InstallMethod { get; set; }
    
    [JsonPropertyName("latest")]
    public bool Latest { get; set; }
    
    [JsonPropertyName("recommended")]
    public bool Recommended { get; set; }
    
    [JsonPropertyName("approved")]
    public bool Approved { get; set; }
    
    [JsonPropertyName("dateModified")]
    public DateTime DateModified { get; set; }
    
    [JsonPropertyName("mavenVersionString")]
    public string? MavenVersionString { get; set; }
    
    [JsonPropertyName("versionJson")]
    public string? VersionJson { get; set; }
    
    [JsonPropertyName("librariesInstallLocation")]
    public string? LibrariesInstallLocation { get; set; }
    
    [JsonPropertyName("minecraftVersion")]
    public string? MinecraftVersion { get; set; }
    
    [JsonPropertyName("additionalFilesJson")]
    public string? AdditionalFilesJson { get; set; }
    
    [JsonPropertyName("modLoaderGameVersionId")]
    public int ModloaderGameVersionId { get; set; }
    
    [JsonPropertyName("modLoaderGameVersionTypeId")]
    public int ModloaderGameVersionTypeId { get; set; }
    
    [JsonPropertyName("modLoaderGameVersionStatus")]
    public int ModloaderGameVersionStatus { get; set; }
    
    [JsonPropertyName("modLoaderGameVersionTypeStatus")]
    public int ModloaderGameVersionTypeStatus { get; set; }
    
    [JsonPropertyName("mcGameVersionId")]
    public int McGameVersionId { get; set; }
    
    [JsonPropertyName("mcGameVersionTypeId")]
    public int McGameVersionTypeId { get; set; }
    
    [JsonPropertyName("mcGameVersionStatus")]
    public int McGameVersionStatus { get; set; }
    
    [JsonPropertyName("mcGameVersionTypeStatus")]
    public int McGameVersionTypeStatus { get; set; }
    
    [JsonPropertyName("installProfileJson")]
    public string? InstallProfileJson { get; set; }
}