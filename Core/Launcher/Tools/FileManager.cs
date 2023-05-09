using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using SimpleLogger;

namespace Launcher.Tools;

internal static class FileManager
{
    public static async Task<bool> StartProcess(string fileName, string? args, Action? started = null,
        Action? exitedAction = null, CancellationToken cancellationToken = default)
    {
        try
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
            };

            var result = process.Start();
            
            started?.Invoke();

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errors = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken); // need to avoid game stuck at loading screen

            Logger.Log($"Exit code: {process.ExitCode}");
            Logger.Log($"Output: {output}");
            Logger.Log($"Errors: {errors}");

            return result && process.ExitCode == 0;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return false;
        }
    }

    public static string? GetFullPath(string? source)
    {
        try
        {
            if (string.IsNullOrEmpty(source))
                return null;

            return Path.GetFullPath(source);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    public static string? GetPathDirectory(string? source)
    {
        try
        {
            if (string.IsNullOrEmpty(source))
                return null;
            
            return Path.GetDirectoryName(source);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    public static bool ExtractToDirectory(string? sourceArchive, string? destinationDirectory)
    {
        try
        {
            if (string.IsNullOrEmpty(sourceArchive) || string.IsNullOrEmpty(destinationDirectory))
                return false;
            
            ZipFile.ExtractToDirectory(sourceArchive, destinationDirectory, overwriteFiles: true);
            return true;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return false;
        }
    }

    public static ZipArchive? OpenZip(string? path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return null;
            
            return ZipFile.Open(path, ZipArchiveMode.Read);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
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
        try
        {
            if (string.IsNullOrEmpty(path))
                return Array.Empty<DirectoryInfo>();

            return Directory.GetDirectories(path).Select(dir => new DirectoryInfo(dir)).ToArray();
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return Array.Empty<DirectoryInfo>();
        }
    }
    
    public static IReadOnlyList<FileInfo> GetFileInfos(string? path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return Array.Empty<FileInfo>();

            return Directory.GetFiles(path).Select(file => new FileInfo(file)).ToArray();
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return Array.Empty<FileInfo>();
        }
    }

    public static bool Delete(string? path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return false;
            
            if (File.Exists(path))
                File.Delete(path);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            return true;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return false;
        }
    }

    public static bool CreateDirectory(string? directory)
    {
        try
        {
            if (string.IsNullOrEmpty(directory))
                return false;
            
            Directory.CreateDirectory(directory);
            return true;
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return false;
        }
    }

    public static string? ComputeSha1(string? fileName)
    {
        try
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
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    public static async Task<string?> ReadFile(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            if (!File.Exists(fileName))
                return null;
            
            using var streamReader = new StreamReader(fileName);
            return await streamReader.ReadToEndAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    public static async Task<bool> WriteFile(string? path, string? content)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(content))
            return false;
        
        await File.WriteAllTextAsync(path, content);
        return true;
    }
}