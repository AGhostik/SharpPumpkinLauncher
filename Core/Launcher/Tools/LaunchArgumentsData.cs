using System.Security.Cryptography;
using System.Text;
using JsonReader.PublicData.Game;
using Launcher.Data;
using Launcher.PublicData;
using SimpleLogger;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsData
{
    public LaunchArgumentsData(LaunchData launchData, MinecraftData minecraftData, MinecraftLaunchFiles launchFiles,
        MinecraftPaths minecraftPaths)
    {
        var isValid = true;
        
        PlayerName = launchData.PlayerName;
        
        Width = launchData.ScreenWidth.ToString();
        Height = launchData.ScreenHeight.ToString();

        Features = new Features(launchData.UseCustomResolution);
        
        VersionId = minecraftData.Id;
        VersionType = minecraftData.Type;
        AssetsVersion = minecraftData.AssetsVersion;
        
        JavaFile = launchFiles.Java;
        ClientJar = launchFiles.Client;
        LoggingFile = launchFiles.Logging;
        Libraries = launchFiles.LibraryFiles;

        LoggingArgument = minecraftData.LoggingData?.Argument ?? string.Empty;
        
        GameDirectory = FileManager.GetFullPath(minecraftPaths.GameDirectory) ?? string.Empty;
        
        AssetsDirectory = minecraftData.IsLegacyAssets() ?
            FileManager.GetFullPath(minecraftPaths.AssetsLegacyDirectory) ?? string.Empty :
            FileManager.GetFullPath(minecraftPaths.AssetsDirectory) ?? string.Empty;
        
        LibrariesDirectory = FileManager.GetFullPath(minecraftPaths.LibrariesDirectory) ?? string.Empty;
        
        NativesDirectory = FileManager.GetFullPath(minecraftPaths.NativesDirectory) ?? string.Empty;

        if (string.IsNullOrEmpty(ClientJar) ||
            string.IsNullOrEmpty(GameDirectory) ||
            string.IsNullOrEmpty(AssetsDirectory) ||
            string.IsNullOrEmpty(LibrariesDirectory) ||
            string.IsNullOrEmpty(NativesDirectory))
        {
            isValid = false;
        }
       
        LauncherName = "\"mclauncher\"";
        AuthAccessToken = "null";
        ClientId = "null";
        AuthXuid = "null";
        AuthUuid = GetUuid(PlayerName) ?? "null";
        UserType = "mojang";

        IsValid = isValid;
    }
    
    public bool IsValid { get; }
    
    public string JavaFile { get; }
    
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
    
    public Features Features { get; }
    
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
            Logger.Log(e);
            return null;
        }
    }
}