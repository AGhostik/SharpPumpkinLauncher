using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Ionic.Zip;
using Microsoft.Win32;

namespace MCLauncher.Model.Managers;

public sealed class FileManager : IFileManager
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
    }

    public string GetPathFilename(string source)
    {
        return Path.GetFileName(source);
    }

    public string? GetPathDirectory(string source)
    {
        return Path.GetDirectoryName(source);
    }

    public void ExtractToDirectory(string sourceArchive, string destinationDirectory)
    {
        using var zip = ZipFile.Read(sourceArchive);
        foreach (var entry in zip)
            entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);
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

    public void StartProcess(string fileName)
    {
        Process.Start(fileName);
    }

    public void CreateDirectory(string directory)
    {
        Directory.CreateDirectory(directory);
    }

    public string GetJavawPath()
    {
        using var baseKey = Registry.LocalMachine.OpenSubKey(ModelResource.JavaKey);
        if (baseKey == null)
            return string.Empty;

        var currentVersion = baseKey.GetValue("CurrentVersion")?.ToString();
        using var homeKey = Registry.LocalMachine.OpenSubKey($"{ModelResource.JavaKey}\\{currentVersion}");
        if (homeKey == null)
            return string.Empty;

        var java = $"{homeKey.GetValue("JavaHome")}\\bin\\javaw.exe";

        return java;
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
}