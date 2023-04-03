using System.Diagnostics;
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
        PlayerName = playerName;
        AssetsVersion = minecraftData.AssetsVersion;

        if (minecraftData.LoggingData?.Argument != null)
            LoggingArgument = minecraftData.LoggingData.Argument;
        else
            LoggingArgument = string.Empty;

        if (fileList.Logging != null)
        {
            var path = FileManager.GetFullPath(fileList.Logging.FileName);
            LoggingFile = !string.IsNullOrEmpty(path) ? $"\"{path}\"" : string.Empty;
        }
        else
            LoggingFile = string.Empty;

        var isValid = true;

        var clientJar = FileManager.GetFullPath(fileList.Client.FileName);
        var gameDirectory = FileManager.GetFullPath(minecraftPaths.GameDirectory);
        var assetsDirectory = minecraftData.IsLegacyAssets() ?
            FileManager.GetFullPath(minecraftPaths.AssetsLegacyDirectory) :
            FileManager.GetFullPath(minecraftPaths.AssetsDirectory);
        var librariesDirectory = FileManager.GetFullPath(minecraftPaths.LibrariesDirectory);
        var nativesDirectory = FileManager.GetFullPath(minecraftPaths.NativesDirectory);

        if (string.IsNullOrEmpty(clientJar) || string.IsNullOrEmpty(gameDirectory) ||
            string.IsNullOrEmpty(assetsDirectory) || string.IsNullOrEmpty(librariesDirectory) ||
            string.IsNullOrEmpty(nativesDirectory))
        {
            isValid = false;
            clientJar = string.Empty;
            gameDirectory = string.Empty;
            assetsDirectory = string.Empty;
            librariesDirectory = string.Empty;
            nativesDirectory = string.Empty;
        }
        
        ClientJar = clientJar;
        GameDirectory = gameDirectory;
        AssetsDirectory = assetsDirectory;
        LibrariesDirectory = librariesDirectory;
        NativesDirectory = nativesDirectory;

        var lib = new List<string>(fileList.LibraryFiles.Count);
        for (var i = 0; i < fileList.LibraryFiles.Count; i++)
        {
            if (fileList.LibraryFiles[i].NeedUnpack)
                continue;
            
            var path = FileManager.GetFullPath(fileList.LibraryFiles[i].FileName);
            if (string.IsNullOrEmpty(path))
            {
                isValid = false;
                break;
            }
            
            lib.Add(path);
        }

        Libraries = lib;

        LauncherName = "\"mclauncher\"";
        AuthAccessToken = "null";
        ClientId = "null";
        AuthXuid = "null";
        AuthUuid = GetUuid(PlayerName) ?? "null";
        UserType = "mojang";
        
        Width = "1200";
        Height = "720";

        IsValid = isValid;
    }
    
    public bool IsValid { get; }
    
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
    
    private static string? GetUuid(string? nickname)
    {
        try
        {
            if (string.IsNullOrEmpty(nickname))
                return null;
            
            var data = MD5.HashData(Encoding.Default.GetBytes(nickname));
            var guid = new Guid(data);
            return guid.ToString();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }
    }
}