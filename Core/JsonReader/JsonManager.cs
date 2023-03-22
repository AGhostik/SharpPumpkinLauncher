using JsonReader.Assets;
using JsonReader.Game;
using JsonReader.Manifest;
using System.Text.Json;

namespace JsonReader;

public sealed class JsonManager
{
    public Versions GetVersions(string? json)
    {
        var result = new Versions();

        if (string.IsNullOrEmpty(json))
            return result;
        
        var manifest = JsonSerializer.Deserialize<ManifestData>(json);

        if (manifest != null)
        {
            if (manifest.Versions != null)
            {
                for (var i = 0; i < manifest.Versions.Length; i++)
                {
                    var version = manifest.Versions[i];
                    if (version.Id == null || version.Url == null || version.Sha1 == null || version.Type == null)
                        continue;

                    result.AddMinecraftVersion(version.Id, version.Url, version.Sha1, version.Type);
                }
            }

            if (manifest.Latest != null)
            {
                result.Latest = manifest.Latest.Release;
                result.LatestSnapshot = manifest.Latest.Snapshoot;
            }
        }

        return result;
    }

    public MinecraftVersionData? GetVersionData(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        
        return JsonSerializer.Deserialize<MinecraftVersionData>(json);
    }
    
    public AssetsData? GetAssetsData(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        
        return JsonSerializer.Deserialize<AssetsData>(json);
    }
}