using System.Text;
using JsonReader.PublicData.Forge;
using Launcher.PublicData;
using Launcher.Tools;
using SimpleLogger;

namespace Launcher.Forge;

internal class ForgeProfileInstaller
{
    /// <summary>
    /// library name - library path
    /// </summary>
    private readonly Dictionary<string, string> _libraries = new();

    private string? _lzmaFile;

    public event Action<ForgeInstallProfileProgress>? Progress;

    public async Task<bool> Install(ForgeInfo forgeInfo, string jre, string minecraftJar, string librariesDirectory,
        CancellationToken cancellationToken)
    {
        var forgeInstall = forgeInfo.ForgeInstall;
        if (forgeInstall.Processors == null || forgeInstall.Data == null)
            return true;

        for (var i = 0; i < forgeInstall.Libraries.Count; i++)
        {
            var library = forgeInstall.Libraries[i];
            if (library.File == null)
                continue;
            
            var fullPath = FileManager.GetFullPath($"{librariesDirectory}\\{library.File.Path}");
            if (string.IsNullOrEmpty(fullPath))
                continue;
            
            if (library.Name.Contains("lzma"))
                _lzmaFile = fullPath;
            
            _libraries.Add(library.Name, fullPath);
        }

        var isSuccess = true;

        if (string.IsNullOrEmpty(_lzmaFile))
        {
            isSuccess = false;
        }
        else
        {
            var client = forgeInstall.Data.Side.Client;
            var processorsCount = forgeInstall.Processors.Count;
            for (var i = 0; i < processorsCount; i++)
            {
                var processor = forgeInstall.Processors[i];

                if (processor.Sides != null && !processor.Sides.Contains(client))
                    continue;

                var arguments = GetArguments(minecraftJar, librariesDirectory, processor, forgeInstall.Data);
                if (string.IsNullOrEmpty(arguments))
                {
                    isSuccess = false;
                    break;
                }

                Progress?.Invoke(new ForgeInstallProfileProgress(i, processorsCount));

                var result = await FileManager.StartProcess(jre, arguments, cancellationToken: cancellationToken);
                if (!result)
                {
                    isSuccess = false;
                    break;
                }
            }
        }

        _libraries.Clear();
        return isSuccess;
    }

    private string? GetArguments(string minecraftJar, string librariesDirectory, ForgeInstallProcessor processor, 
        ForgeInstallData forgeInstallData)
    {
        var sb = new StringBuilder();
        sb.Append("-cp ");
        
        if (!_libraries.TryGetValue(processor.Jar, out var mainJarPath))
            return null;
        
        sb.Append(mainJarPath);
        sb.Append(';');
        
        for (var i = 0; i < processor.Classpath.Count; i++)
        {
            if (!_libraries.TryGetValue(processor.Classpath[i], out var classpathPath))
            {
                Logger.Log($"Cant find library path for '{processor.Classpath[i]}'");
                continue;
            }
            
            sb.Append(classpathPath);
            if (i != processor.Classpath.Count - 1)
                sb.Append(';');
        }

        sb.Append(' ');

        var mainClass = GetMainClassFromJar(mainJarPath);
        if (string.IsNullOrEmpty(mainClass))
            return null;
        
        sb.Append(mainClass);
        sb.Append(' ');
        
        for (var i = 0; i < processor.Args.Count; i++)
        {
            var arg = processor.Args[i];

            if (arg.StartsWith("--"))
            {
                sb.Append(arg);
                sb.Append(' ');
                continue;
            }

            if (arg.StartsWith('[') && arg.EndsWith(']'))
            {
                var libraryName = arg.Substring(1, arg.Length - 2);
                if (_libraries.TryGetValue(libraryName, out var libraryPath))
                {
                    sb.Append(libraryPath);
                    sb.Append(' ');
                }
                continue;
            }

            if (arg.StartsWith('{') && arg.EndsWith('}'))
            {
                var path = ReplaceWellKnownArgumentKey(arg, minecraftJar, librariesDirectory, forgeInstallData);
                if (!string.IsNullOrEmpty(path))
                {
                    sb.Append(path);
                    sb.Append(' ');
                }
                
                continue;
            }

            sb.Append(arg);
            sb.Append(' ');
        }

        return sb.ToString();
    }

    private string? ReplaceWellKnownArgumentKey(string arg, string minecraftJar, string librariesDirectory,
        ForgeInstallData forgeInstallData)
    {
        switch (arg)
        {
            case "{MAPPINGS}":
                return GetPathFromPackageName(forgeInstallData.Mappings.Client, librariesDirectory);
            case "{MOJMAPS}":
                return GetPathFromPackageName(forgeInstallData.Mojmaps.Client, librariesDirectory);
            case "{MERGED_MAPPINGS}":
                return GetPathFromPackageName(forgeInstallData.MergedMappings.Client, librariesDirectory);
            case "{MC_UNPACKED}":
                return GetPathFromPackageName(forgeInstallData.McUnpacked.Client, librariesDirectory);
            case "{MC_SLIM}":
                return GetPathFromPackageName(forgeInstallData.McSlim.Client, librariesDirectory);
            case "{MC_EXTRA}":
                return GetPathFromPackageName(forgeInstallData.McExtra.Client, librariesDirectory);
            case "{MC_SRG}":
                return GetPathFromPackageName(forgeInstallData.McSrg.Client, librariesDirectory);
            case "{PATCHED}":
                return GetPathFromPackageName(forgeInstallData.Patched.Client, librariesDirectory);
            case "{MCP_VERSION}":
                return GetPathFromPackageName(forgeInstallData.MpcVersion.Client, librariesDirectory);
            case "{SIDE}":
                return "client";
            case "{MINECRAFT_JAR}":
                return minecraftJar;
            case "{BINPATCH}":
                return _lzmaFile;
            
            case "{MC_SLIM_SHA}":
            case "{MC_EXTRA_SHA}":
            case "{_PATCHED_SHA}":
                Logger.Log($"SHA argument - '{arg}', dont know what to do");
                return null;

            default:
                Logger.Log($"Unknown Forge Install processor arg: '{arg}'");
                return null;
        }
    }
    
    private static string? GetMainClassFromJar(string? mainJarPath)
    {
        using var zip = FileManager.OpenZip(mainJarPath);

        var mainfest = zip?.GetEntry("META-INF/MANIFEST.MF");
        if (mainfest == null)
            return null;
        
        var stream = new StreamReader(mainfest.Open());

        while (stream.ReadLine() is { } currentLine)
        {
            if (!currentLine.Contains("Main-Class:"))
                continue;
            
            stream.Close();
            return currentLine.Replace("Main-Class:", "").Trim();
        }
        
        stream.Close();
        return null;
    }

    public static string? GetPathFromPackageName(string name, string librariesDirectory)
    {
        var libName = GetLibraryFileName(name.Replace("[", "").Replace("]", ""));
        if (string.IsNullOrEmpty(libName))
            return null;
        
        return $"{librariesDirectory}\\{libName}";
    }
    
    private static string? GetLibraryFileName(string name)
    {
        var extension = ".jar";
        if (name.Contains('@'))
        {
            extension = $".{name.Substring(name.LastIndexOf('@') + 1)}";
            name = name.Substring(0, name.LastIndexOf('@'));
        }

        var targets = name.Split(':');
        if (targets.Length < 3) 
            return null;
        
        var pathBase = 
            string.Join("\\", targets[0].Replace('.', '\\'), targets[1], targets[2], targets[1]) + '-' + targets[2];

        if (targets.Length == 4)
        {
            return $"{pathBase}-{targets[3]}{extension}";
        }

        return $"{pathBase}{extension}";
    }

}