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
    
    public static async Task<string> DownloadJsonAsync(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(new Uri(url));
        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
    
    public static async Task<string> DownloadFile(string url, CancellationToken cancellationToken, Action<long>? bytesReceived = null)
    {
        using var client = new HttpClient();
        using var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).Result;
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            
        var totalRead = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;
        var sb = new StringBuilder();

        do
        {
            var read = await contentStream.ReadAsync(buffer, cancellationToken);
            if (read == 0)
            {
                isMoreToRead = false;
            }
            else
            {
                sb.Append(Encoding.Default.GetString(buffer, 0, read));
                totalRead += read;
                bytesReceived?.Invoke(totalRead);
            }
        } while (isMoreToRead);

        return sb.ToString();
    }

    public static async Task DownloadFilesParallel(IEnumerable<(Uri source, string filename)> download,
        Action<long>? bytesReceived = null)
    {
        var totalRead = 0L;
        
        await Parallel.ForEachAsync(
            download,
            new ParallelOptions { MaxDegreeOfParallelism = 10 },
            DownloadFileParallel);
        
        async ValueTask DownloadFileParallel((Uri source, string filename) data, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            using var response = client
                .GetAsync(data.source, HttpCompletionOption.ResponseHeadersRead, cancellationToken).Result;
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(data.filename, FileMode.Create, FileAccess.Write,
                FileShare.None, 8192, true);
            
            var buffer = new byte[8192];
            var isMoreToRead = true;

            do
            {
                var read = await contentStream.ReadAsync(buffer, cancellationToken);
                if (read == 0)
                {
                    isMoreToRead = false;
                }
                else
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);

                    totalRead += read;
                    bytesReceived?.Invoke(totalRead);
                }
            } while (isMoreToRead);
        }
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