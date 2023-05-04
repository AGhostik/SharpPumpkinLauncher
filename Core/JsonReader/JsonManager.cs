using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using JsonReader.InternalData.Assets;
using JsonReader.InternalData.Forge;
using JsonReader.InternalData.Game;
using JsonReader.InternalData.Manifest;
using JsonReader.InternalData.Runtime;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Forge;
using JsonReader.PublicData.Game;
using JsonReader.PublicData.Manifest;
using JsonReader.PublicData.Runtime;
using SimpleLogger;

namespace JsonReader;

public sealed class JsonManager
{
    public ForgeInfo? GetForgeInfo(string? json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var forgeParentData = JsonSerializer.Deserialize<ForgeParentData>(json);
            var forgeData = forgeParentData?.Data;
            if (forgeData == null)
                return null;

            if (string.IsNullOrEmpty(forgeData.VersionJson))
                return null;

            var forgeVersionJson = forgeData.VersionJson.Replace("\\r\\n", string.Empty).Replace("\\\"", "\"");
            var forgeVersionData = JsonSerializer.Deserialize<ForgeVersionData>(forgeVersionJson);
            if (forgeVersionData == null)
                return null;
            
            var libraries = GetLibraries(forgeVersionData.Libraries);

            if (string.IsNullOrEmpty(forgeVersionData.MinecraftArguments))
                return null;

            if (string.IsNullOrEmpty(forgeVersionData.MainClass))
                return null;

            return new ForgeInfo(forgeVersionData.MainClass, forgeVersionData.MinecraftArguments, libraries);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }
    
    public ForgeVersions? GetForgeVersions(string? json, string curseForgeUrl)
    {
        const int forgeType = 1;
        
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var forgeManifestsData = JsonSerializer.Deserialize<ForgeManifestsData>(json);
            if (forgeManifestsData?.Data == null)
                return null;

            ForgeVersion? recommended = null;
            ForgeVersion? latest = null;
            var versions = new List<ForgeVersion>();
            for (var i = 0; i < forgeManifestsData.Data.Length; i++)
            {
                var manifestData = forgeManifestsData.Data[i];
                
                if (string.IsNullOrEmpty(manifestData.Name) || string.IsNullOrEmpty(manifestData.GameVersion))
                    continue;
                
                if (manifestData.Type != forgeType)
                    continue;

                var url = $"{curseForgeUrl}\\{manifestData.Name}";
                var forgeVersion = new ForgeVersion(manifestData.Name, url, manifestData.GameVersion);

                if (manifestData.Recommended)
                    recommended = forgeVersion;

                if (manifestData.Latest)
                    latest = forgeVersion;
                
                versions.Add(forgeVersion);
            }

            return new ForgeVersions(latest, recommended, versions);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }
    
    public RuntimeFiles? GetRuntimeFiles(string? json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var allRuntimesData = JsonSerializer.Deserialize<RuntimeFilesData>(json);

            if (allRuntimesData?.Files == null)
                return null;

            var files = new List<RuntimeFile>();

            foreach (var (path, data) in allRuntimesData.Files)
            {
                if (data.Type == "directory")
                    continue;
                
                if (data.Downloads == null)
                    continue;

                var lzma = data.Downloads.Lzma;
                var raw = data.Downloads.Raw;

                RuntimeFile runtimeFile;
                if (raw != null && !string.IsNullOrEmpty(raw.Sha1) && !string.IsNullOrEmpty(raw.Url))
                {
                    runtimeFile = new RuntimeFile(path, raw.Sha1, raw.Size, raw.Url);
                }
                else
                {
                    if (lzma == null || string.IsNullOrEmpty(lzma.Sha1) || string.IsNullOrEmpty(lzma.Url))
                        continue;

                    runtimeFile = new RuntimeFile(path, lzma.Sha1, lzma.Size, lzma.Url);
                }

                files.Add(runtimeFile);
            }

            return new RuntimeFiles(files);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }
    
    public OsRuntimes? GetAllRuntimes(string? json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var allRuntimesData = JsonSerializer.Deserialize<AllRuntimesData>(json);

            if (allRuntimesData == null ||
                allRuntimesData.Linux == null || allRuntimesData.LinuxI386 == null ||
                allRuntimesData.MacOs == null || allRuntimesData.MacOsArm64 == null ||
                allRuntimesData.WindowsArm64 == null ||
                allRuntimesData.Windows64 == null || allRuntimesData.Windows86 == null)
                return null;

            var linux = GetOsRuntime(allRuntimesData.Linux);
            var linuxI386 = GetOsRuntime(allRuntimesData.LinuxI386);
            var macOs = GetOsRuntime(allRuntimesData.MacOs);
            var macOsArm64 = GetOsRuntime(allRuntimesData.MacOsArm64);
            var windowsArm64 = GetOsRuntime(allRuntimesData.WindowsArm64);
            var windows64 = GetOsRuntime(allRuntimesData.Windows64);
            var windows86 = GetOsRuntime(allRuntimesData.Windows86);
            var result = new OsRuntimes(linux, linuxI386, macOs, macOsArm64, windowsArm64, windows64, windows86);

            return result;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    private static OsRuntime GetOsRuntime(RuntimesData? runtimesData)
    {
        var javaRuntimeAlpha = GetRuntime(runtimesData?.JavaRuntimeAlpha);
        var javaRuntimeBeta = GetRuntime(runtimesData?.JavaRuntimeBeta);
        var javaRuntimeGamma = GetRuntime(runtimesData?.JavaRuntimeGamma);
        var jreLegacy = GetRuntime(runtimesData?.JreLegacy);
        var minecraftJavaExe = GetRuntime(runtimesData?.MinecraftJavaExe);

        return new OsRuntime(javaRuntimeAlpha, javaRuntimeBeta, javaRuntimeGamma, jreLegacy, minecraftJavaExe);
    }

    private static Runtime? GetRuntime(IReadOnlyList<RuntimeData>? runtimeData)
    {
        if (runtimeData == null || runtimeData.Count <= 0)
            return null;
        
        var javaRuntime = runtimeData[0];

        if (javaRuntime.Version != null && !string.IsNullOrEmpty(javaRuntime.Version.Name) &&
            javaRuntime.Manifest != null && !string.IsNullOrEmpty(javaRuntime.Manifest.Sha1) &&
            !string.IsNullOrEmpty(javaRuntime.Manifest.Url))
        {
            return new Runtime(javaRuntime.Version.Name, javaRuntime.Version.Released,
                javaRuntime.Manifest.Sha1, javaRuntime.Manifest.Size, javaRuntime.Manifest.Url);
        }

        return null;
    }
    
    public IReadOnlyList<Asset>? GetAssets(string? json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var assetsData = JsonSerializer.Deserialize<AssetsData>(json);

            if (assetsData?.AssetList == null)
                return null;

            var hashs = new HashSet<string>();
            var result = new List<Asset>(assetsData.AssetList.Count);
            foreach (var assetData in assetsData.AssetList)
            {
                if (string.IsNullOrEmpty(assetData.Value.Hash))
                    continue;

                if (hashs.Contains(assetData.Value.Hash))
                    continue;

                result.Add(new Asset(assetData.Key, assetData.Value.Hash, assetData.Value.Size));
                hashs.Add(assetData.Value.Hash);
            }

            return result;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }
    
    public Versions? GetVersions(string? json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var manifest = JsonSerializer.Deserialize<ManifestData>(json);

            if (manifest == null)
                return null;

            var result = new Versions();
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

            return result;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    public MinecraftData? GetMinecraftData(string? json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var minecraftVersionData = JsonSerializer.Deserialize<MinecraftVersionData>(json);

            if (minecraftVersionData == null)
                return null;

            if (minecraftVersionData.AssetIndex == null || string.IsNullOrEmpty(minecraftVersionData.AssetIndex.Url) ||
                string.IsNullOrEmpty(minecraftVersionData.AssetIndex.Sha1))
            {
                return null;
            }

            var assetsIndex = new DownloadFile(minecraftVersionData.AssetIndex.Sha1,
                minecraftVersionData.AssetIndex.Size,
                minecraftVersionData.AssetIndex.Url);

            if (string.IsNullOrEmpty(minecraftVersionData.Id) || string.IsNullOrEmpty(minecraftVersionData.Type) ||
                string.IsNullOrEmpty(minecraftVersionData.Assets) ||
                string.IsNullOrEmpty(minecraftVersionData.MainClass))
            {
                return null;
            }

            if (minecraftVersionData.Downloads?.Client == null)
                return null;

            if (!TryGetDownloadFile(minecraftVersionData.Downloads.Client, out var client))
                return null;

            DownloadFile? server = null;
            if (minecraftVersionData.Downloads.Server != null &&
                !TryGetDownloadFile(minecraftVersionData.Downloads.Server, out server))
                return null;

            var loggingData = GetLoggingData(minecraftVersionData);

            if (!TryGetArguments(minecraftVersionData, out var arguments))
                return null;

            if (minecraftVersionData.JavaVersion == null ||
                string.IsNullOrEmpty(minecraftVersionData.JavaVersion.Component))
                return null;

            var javaVersion = new JavaVersion(minecraftVersionData.JavaVersion.Component,
                minecraftVersionData.JavaVersion.MajorVersion);

            var libraries = GetLibraries(minecraftVersionData.Libraries);

            return new MinecraftData(
                id: minecraftVersionData.Id,
                type: minecraftVersionData.Type,
                assetsVersion: minecraftVersionData.Assets,
                mainClass: minecraftVersionData.MainClass,
                minimumLauncherVersion: minecraftVersionData.MinimumLauncherVersion,
                releaseTime: minecraftVersionData.ReleaseTime,
                time: minecraftVersionData.Time,
                client,
                server,
                assetsIndex,
                loggingData,
                arguments,
                javaVersion,
                libraries);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    private static IReadOnlyList<Library> GetLibraries(IReadOnlyList<LibraryData>? libraryDatas)
    {
        if (libraryDatas == null)
            return Array.Empty<Library>();
        
        var libraries = new List<Library>(libraryDatas.Count);
        for (var i = 0; i < libraryDatas.Count; i++)
        {
            var lib = libraryDatas[i];
            var downloads = lib.Downloads;
            if (downloads == null)
                continue;

            LibraryFile? file = null;
            LibraryFile? nativesWindowsFile = null;
            LibraryFile? nativesLinuxFile = null;
            LibraryFile? nativesOsxFile = null;
            
            if (downloads.Artifact != null)
            {
                file = GetLibraryFile(downloads.Artifact);
            }
            
            if (downloads.Classifiers != null)
            {
                nativesWindowsFile = GetLibraryFile(downloads.Classifiers.NativesWindows);
                nativesLinuxFile = GetLibraryFile(downloads.Classifiers.NativesLinux);
                nativesOsxFile = GetLibraryFile(downloads.Classifiers.NativesOsx);
            }
            
            if (file == null && nativesWindowsFile == null && nativesLinuxFile == null && nativesOsxFile == null)
                continue;

            var rules = new List<Rule>(0);
            if (lib.Rules != null)
            {
                rules.Capacity = lib.Rules.Length;
                for (var j = 0; j < lib.Rules.Length; j++)
                {
                    var ruleData = lib.Rules[j];
                    if (TryGetRule(ruleData, out var rule))
                        rules.Add(rule);
                }
            }

            var delete = new List<string>();
            if (lib.Extract?.Exclude != null)
            {
                for (var j = 0; j < lib.Extract.Exclude.Length; j++)
                {
                    var value = lib.Extract.Exclude[j];
                    if (!string.IsNullOrEmpty(value))
                        delete.Add(value);
                }
            }
            
            var library = new Library(file, nativesWindowsFile, nativesLinuxFile, nativesOsxFile,
                lib.Natives?.Windows, lib.Natives?.Linux, lib.Natives?.Osx, rules, delete);
            
            libraries.Add(library);
        }

        return libraries;
    }

    private static LibraryFile? GetLibraryFile(ArtifactData? artifact)
    {
        if (artifact == null)
            return null;

        if (string.IsNullOrEmpty(artifact.Path) || string.IsNullOrEmpty(artifact.Sha1) ||
            string.IsNullOrEmpty(artifact.Url))
            return null;
            
        return new LibraryFile(artifact.Path, artifact.Sha1, artifact.Size, artifact.Url);
    }

    private static Logging? GetLoggingData(MinecraftVersionData minecraftVersionData)
    {
        if (minecraftVersionData.Logging?.Client == null)
            return null;
        
        var argument = minecraftVersionData.Logging.Client.Argument;
        var file = minecraftVersionData.Logging.Client.File;

        if (file != null &&
            !string.IsNullOrEmpty(file.Sha1) && !string.IsNullOrEmpty(file.Url) && !string.IsNullOrEmpty(argument))
        {
            return new Logging(argument, new DownloadFile(file.Sha1, file.Size, file.Url));
        }

        return null;
    }
    
    private static bool TryGetDownloadFile(DownloadData downloadData, [NotNullWhen(true)] out DownloadFile? downloadFile)
    {
        if (string.IsNullOrEmpty(downloadData.Sha1) || string.IsNullOrEmpty(downloadData.Url))
        {
            downloadFile = null;
            return false;
        }
        
        downloadFile = new DownloadFile(downloadData.Sha1, downloadData.Size, downloadData.Url);
        return true;
    }

    private static bool TryGetArguments(MinecraftVersionData minecraftVersionData, [NotNullWhen(true)] out Arguments? arguments)
    {
        if (minecraftVersionData.Arguments != null)
        {
            var gameArguments = GetArgumentItems(minecraftVersionData.Arguments.Game);
            var jvmArguments = GetArgumentItems(minecraftVersionData.Arguments.Jvm);

            arguments = new Arguments(gameArguments, jvmArguments);
            return true;
        }

        if (!string.IsNullOrEmpty(minecraftVersionData.MinecraftArguments))
        {
            arguments = GetLegacyArguments(minecraftVersionData.MinecraftArguments);
            return true;
        }

        arguments = null;
        return false;
    }

    private static Arguments GetLegacyArguments(string minecraftArguments)
    {
        var legacyArguments = new LegacyArguments(minecraftArguments,
            "-Djava.library.path=${natives_directory} -cp ${classpath}");
            
        return new Arguments(legacyArguments);
    }

    private static List<ArgumentItem> GetArgumentItems(IReadOnlyList<ArgumentItemData>? argumentDatas)
    {
        List<ArgumentItem> gameArguments;
        if (argumentDatas != null)
        {
            gameArguments = new List<ArgumentItem>(argumentDatas.Count);
            for (var i = 0; i < argumentDatas.Count; i++)
            {
                var gameArg = argumentDatas[i];
                if (gameArg.Value == null)
                    continue;

                var values = new List<string>(gameArg.Value.Length);
                for (var j = 0; j < gameArg.Value.Length; j++)
                {
                    var value = gameArg.Value[j];
                    if (string.IsNullOrEmpty(value))
                        continue;
                    
                    if (value.Contains("Windows 10"))
                        value = value.Replace("Windows 10", "\"Windows 10\"");
                    
                    values.Add(value);
                }
                
                var rules = new List<Rule>();
                if (gameArg.Rules != null)
                {
                    for (var j = 0; j < gameArg.Rules.Length; j++)
                    {
                        var ruleData = gameArg.Rules[j];
                        
                        if (!TryGetRule(ruleData, out var rule))
                            continue;
                        
                        rules.Add(rule);
                    }
                }

                var argumentItem = new ArgumentItem(values, rules);
                gameArguments.Add(argumentItem);
            }
        }
        else
        {
            gameArguments = new List<ArgumentItem>();
        }

        return gameArguments;
    }

    private static bool TryGetRule(RulesData ruledData, [NotNullWhen(true)] out Rule? rule)
    {
        var action = ruledData.Action;
        if (string.IsNullOrEmpty(action))
        {
            rule = null;
            return false;
        }

        Os? os = null;
        if (ruledData.Os != null)
        {
            os = new Os()
            {
                Architecture = ruledData.Os.Architecture,
                Name = ruledData.Os.Name,
                Version = ruledData.Os.Version
            };
        }
        
        rule = new Rule(action)
        {
            Features = ruledData.Features,
            Os = os
        };

        return true;
    }
}