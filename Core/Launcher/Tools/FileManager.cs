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
            RedirectStandardOutput = true
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
        
        var stdoutx = process.StandardOutput.ReadToEnd();         
        var stderrx = process.StandardError.ReadToEnd();             
        process.WaitForExit();
        
        Debug.WriteLine(stdoutx);
        Debug.WriteLine(stderrx);
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

    public async Task DownloadFile(string url, string fileName)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(new Uri(url));

        await using var fileStream = new FileStream(fileName, FileMode.CreateNew);
        await response.Content.CopyToAsync(fileStream);
    }

    public async Task DownloadFiles(List<Tuple<Uri, string>> urlFileName, Action? downloadedEvent = null)
    {
        using var client = new HttpClient();

        foreach (var (uri, fileName) in urlFileName)
        {
            var response = await client.GetAsync(uri);
            await using var fileStream = new FileStream(fileName, FileMode.CreateNew);
            await response.Content.CopyToAsync(fileStream);
            
            downloadedEvent?.Invoke();
        }
    }
    
    public async Task DownloadFiles(IReadOnlyList<(Uri source, string filename)> download, Action<int>? eachDownloadedEvent = null)
    {
        using var client = new HttpClient();

        for (var i = 0; i < download.Count; i++)
        {
            var (uri, fileName) = download[i];
            
            if (FileExist(fileName))
                continue;

            var stream = await client.GetStreamAsync(uri);
            await using var fileStream = new FileStream(fileName, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);

            eachDownloadedEvent?.Invoke(i);
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