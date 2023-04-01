using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using JsonReader.PublicData.Assets;
using JsonReader.PublicData.Game;
using Launcher.Data;

namespace Launcher.Tools;

internal static class FileManager
{
    public static async Task StartProcess(string fileName, string? args, Action? exitedAction = null)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            Verb = "runas"
        };

        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
        process.Exited += (_, _) =>
        {
            exitedAction?.Invoke();
            Debug.WriteLine(process.ExitCode);
        };

        process.Start();
        
        var output = await process.StandardOutput.ReadToEndAsync();         
        var errors = await process.StandardError.ReadToEndAsync();             
        await process.WaitForExitAsync(); // need to avoid game stuck at loading screen
        
        Debug.WriteLine(output);
        Debug.WriteLine(errors);
    }
    
    public static string GetFileName(string? source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        
        return Path.GetFileName(source);
    }

    public static string GetFullPath(string? source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        
        return Path.GetFullPath(source);
    }

    public static string? GetPathDirectory(string source)
    {
        return Path.GetDirectoryName(source);
    }

    public static void ExtractToDirectory(string sourceArchive, string destinationDirectory)
    {
        ZipFile.ExtractToDirectory(sourceArchive, destinationDirectory, overwriteFiles: true);
    }

    public static bool FileExist(string path)
    {
        return File.Exists(path);
    }
    
    public static bool DirectoryExist(string path)
    {
        return Directory.Exists(path);
    }

    public static IReadOnlyList<DirectoryInfo> GetSubDirectories(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return Array.Empty<DirectoryInfo>();

        return Directory.GetDirectories(path).Select(dir => new DirectoryInfo(dir)).ToArray();
    }
    
    public static IReadOnlyList<FileInfo> GetFileInfos(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return Array.Empty<FileInfo>();

        return Directory.GetFiles(path).Select(file => new FileInfo(file)).ToArray();
    }

    public static void Delete(string path)
    {
        if (FileExist(path)) 
            File.Delete(path);

        if (DirectoryExist(path)) 
            Directory.Delete(path, true);
    }

    public static void CreateDirectory(string directory)
    {
        //todo: catch exceptions
        Directory.CreateDirectory(directory);
    }

    public static string? ComputeSha1(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;
        
        using var fileStream = new FileStream(fileName, FileMode.Open);
        using var bufferedStream = new BufferedStream(fileStream);
        var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(bufferedStream);
        var formatted = new StringBuilder(2 * hash.Length);
        
        foreach (var b in hash)
            formatted.Append($"{b:x2}");

        return formatted.ToString();
    }

    public static async Task<string?> ReadFile(string fileName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        if (!File.Exists(fileName))
            return null;
        
        using var streamReader = new StreamReader(fileName);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public static async Task WriteFile(string? path, string? content)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(content))
            return;
        
        await File.WriteAllTextAsync(path, content);
    }
    
    public static MinecraftFileList GetFileList(MinecraftData data, IReadOnlyList<Asset> assets,
        MinecraftPaths minecraftPaths, string minecraftVersionId)
    {
        var client = new MinecraftFile(data.Client.Url, data.Client.Size, data.Client.Sha1,
            $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}.jar");
        
        var server = new MinecraftFile(data.Server.Url, data.Server.Size, data.Server.Sha1,
            $"{minecraftPaths.VersionDirectory}\\{minecraftVersionId}-server.jar");
        
        var librariesFiles = GetLibrariesFiles(data.Libraries, minecraftPaths);
        var assetsFiles = GetAssetsFiles(assets, minecraftPaths);
        
        var minecraftFileList = new MinecraftFileList(client, server, librariesFiles, assetsFiles);

        if (data.LoggingData != null)
        {
            minecraftFileList.Logging = new MinecraftFile(data.LoggingData.File.Url, data.LoggingData.File.Size,
                data.LoggingData.File.Sha1, $"{minecraftPaths.VersionDirectory}\\log4j2.xml");
        }

        return minecraftFileList;
    }

    private static IReadOnlyList<MinecraftLibraryFile> GetLibrariesFiles(IReadOnlyList<Library> libraries,
        MinecraftPaths minecraftPaths)
    {
        var result = new List<MinecraftLibraryFile>(libraries.Count);
        for (var i = 0; i < libraries.Count; i++)
        {
            var libraryData = libraries[i];
            
            if (!OsRuleManager.IsAllowed(libraryData.Rules))
                continue;

            if (OperatingSystem.IsWindows())
            {
                if (!string.IsNullOrEmpty(libraryData.NativesWindows) && libraryData.NativesWindowsFile != null)
                {
                    result.Add(GetNativeLibraryFile(libraryData.NativesWindowsFile, minecraftPaths.TemporaryDirectory,
                        libraryData.Delete));
                }
            }
            else if (OperatingSystem.IsLinux())
            {
                if (!string.IsNullOrEmpty(libraryData.NativesLinux) && libraryData.NativesLinuxFile != null)
                {
                    result.Add(GetNativeLibraryFile(libraryData.NativesLinuxFile, minecraftPaths.TemporaryDirectory,
                        libraryData.Delete));
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                if (!string.IsNullOrEmpty(libraryData.NativesOsx) && libraryData.NativesOsxFile != null)
                {
                    result.Add(GetNativeLibraryFile(libraryData.NativesOsxFile, minecraftPaths.TemporaryDirectory,
                        libraryData.Delete));
                }
            }

            if (libraryData.File != null)
            {
                var fileName = $"{minecraftPaths.LibrariesDirectory}\\{libraryData.File.Path}";
                var minecraftLibraryFile = new MinecraftLibraryFile(libraryData.File.Url, libraryData.File.Size,
                    libraryData.File.Sha1, fileName);
                result.Add(minecraftLibraryFile);
            }
        }

        return result;
    }

    private static MinecraftLibraryFile GetNativeLibraryFile(LibraryFile file, string temporaryDirectory,
        IReadOnlyList<string> deleteFiles)
    {
        var nativeFileName = $"{temporaryDirectory}\\{file.Path}";
        var minecraftNativeLibraryFile = new MinecraftLibraryFile(file.Url, file.Size, file.Sha1, nativeFileName)
        {
            NeedUnpack = true,
            Delete = deleteFiles
        };
        return minecraftNativeLibraryFile;
    }
    
    private static IReadOnlyList<MinecraftFile> GetAssetsFiles(IReadOnlyList<Asset> assets, MinecraftPaths minecraftPaths)
    {
        var result = new List<MinecraftFile>(assets.Count);
        for (var i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];

            var hashString = asset.Hash;
            
            if (hashString.Length < 2)
                continue;
            
            var subDirectory = $"{hashString[0]}{hashString[1]}";
            var fileName = $"{minecraftPaths.AssetsDirectory}\\objects\\{subDirectory}\\{hashString}";
            var minecraftFile = new MinecraftFile($"{WellKnownUrls.AssetsUrl}/{subDirectory}/{hashString}", asset.Size,
                asset.Hash, fileName);
            
            result.Add(minecraftFile);
        }
        
        return result;
    }
}