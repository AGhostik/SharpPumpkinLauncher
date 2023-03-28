using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;

namespace Launcher.Tools;

internal sealed class FileManager
{
    public void StartProcess(string fileName, string? args, Action? exitedAction = null)
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
            
            // var output = process.StandardOutput.ReadToEnd();
            // var errors = process.StandardError.ReadToEnd();
            //
            // Debug.WriteLine(output);
            // Debug.WriteLine(errors);
        };

        process.Start();
    }

    public string GetPathFilename(string source)
    {
        return Path.GetFileName(source);
    }
    
    public string GetFullPath(string? source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        
        return Path.GetFullPath(source);
    }

    public string? GetPathDirectory(string source)
    {
        return Path.GetDirectoryName(source);
    }

    public void ExtractToDirectory(string sourceArchive, string destinationDirectory)
    {
        ZipFile.ExtractToDirectory(sourceArchive, destinationDirectory, overwriteFiles: true);
    }

    public bool FileExist(string path)
    {
        return File.Exists(path);
    }

    public void Delete(string path)
    {
        if (FileExist(path)) 
            File.Delete(path);

        if (DirectoryExist(path)) 
            Directory.Delete(path, true);
    }

    public bool DirectoryExist(string path)
    {
        return Directory.Exists(path);
    }

    public void CreateDirectory(string directory)
    {
        //todo: catch exceptions
        Directory.CreateDirectory(directory);
    }
    
    public async Task<string> DownloadJsonAsync(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(new Uri(url));
        var json = await response.Content.ReadAsStringAsync();
        return json;
    }

    public async Task DownloadFilesParallel(IEnumerable<(Uri source, string filename)> download, Action<int>? eachDownloadedEvent = null)
    {
        var index = 0;
        
        await Parallel.ForEachAsync(
            download, 
            new ParallelOptions{MaxDegreeOfParallelism = 10},
            DownloadFile);
        
        async ValueTask DownloadFile((Uri source, string filename) data, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            var stream = await client.GetStreamAsync(data.source, cancellationToken);
            await using var fileStream = new FileStream(data.filename, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream, cancellationToken);
            
            eachDownloadedEvent?.Invoke(index);
            index++;
        }
    }

    public string? ComputeSha1(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;
        
        using var fileStream = new FileStream(fileName, FileMode.Open);
        using var bufferedStream = new BufferedStream(fileStream);
        var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(bufferedStream);
        var formatted = new StringBuilder(2 * hash.Length);
        
        foreach (var b in hash)
            formatted.Append($"{b:X2}");

        return formatted.ToString();
    }
}