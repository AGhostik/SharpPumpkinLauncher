using System.Security.Cryptography;
using System.Text;
using JsonReader.PublicData.Game;
using Launcher.Data;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsData
{
    public LaunchArgumentsData(MinecraftData minecraftData, MinecraftFileList fileList, MinecraftPaths minecraftPaths,
        string playerName)
    {
        VersionId = minecraftData.Id;
        VersionType = minecraftData.Type;
        ClientJar = FileManager.GetFullPath(fileList.Client.FileName);
        PlayerName = playerName;

        AssetsVersion = minecraftData.AssetsVersion;

        GameDirectory = FileManager.GetFullPath(minecraftPaths.GameDirectory);
        AssetsDirectory = FileManager.GetFullPath(minecraftPaths.AssetsDirectory);
        LibrariesDirectory = FileManager.GetFullPath(minecraftPaths.LibrariesDirectory);
        NativesDirectory = FileManager.GetFullPath(minecraftPaths.NativesDirectory);

        LoggingArgument = minecraftData.LoggingData?.Argument ?? "null";
        LoggingFile = fileList.Logging == null ? "null" : $"\"{FileManager.GetFullPath(fileList.Logging.FileName)}\"";

        var lib = new List<string>(fileList.LibraryFiles.Count);
        for (var i = 0; i < fileList.LibraryFiles.Count; i++)
        {
            if (fileList.LibraryFiles[i].NeedUnpack)
                continue;
            lib.Add(FileManager.GetFullPath(fileList.LibraryFiles[i].FileName));
        }

        Libraries = lib;

        LauncherName = "\"mclauncher\"";
        AuthAccessToken = "null";
        ClientId = "null";
        AuthXuid = "null";
        AuthUuid = GetUuid(PlayerName);
        UserType = "mojang";
        
        Width = "1200";
        Height = "720";
    }
    
    public string Width { get; }
    public string Height { get; }

    public string VersionId { get; }
    public string VersionType { get; }
    public string ClientJar { get; }
    
    public string PlayerName { get; }
    
    public string AssetsVersion { get; }
        
    public string GameDirectory { get; }
    public string AssetsDirectory { get; }
    public string LibrariesDirectory { get; }
    public string NativesDirectory { get; }
    
    public string LoggingArgument { get; }
    public string LoggingFile { get; }
    
    public string AuthAccessToken { get; }
    public string ClientId { get; }
    public string AuthXuid { get; }
    public string AuthUuid { get; }
    public string UserType { get; }
    public string LauncherName { get; }
    
    public IReadOnlyList<string> Libraries { get; }
    
    private static string GetUuid(string nickname)
    {
        var data = MD5.HashData(Encoding.Default.GetBytes(nickname));
        var guid = new Guid(data);
        return guid.ToString();
    }
}