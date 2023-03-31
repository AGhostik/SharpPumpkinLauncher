using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;

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
}