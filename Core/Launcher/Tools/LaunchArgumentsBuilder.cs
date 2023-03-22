using System.Security.Cryptography;
using System.Text;
using JsonReader.Game;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsBuilder
{
    public static string GetLaunchArguments(MinecraftVersionData minecraftVersionData, LaunchArgumentsData launchArgumentsData)
    {
        if (minecraftVersionData.Arguments != null)
        {
            return GetArguments(launchArgumentsData, minecraftVersionData.Arguments, minecraftVersionData.MainClass,
                minecraftVersionData.MinimumLauncherVersion);
        }

        if (minecraftVersionData.MinecraftArguments != null)
            return GetLegacyArguments(minecraftVersionData.MinecraftArguments);

        throw new Exception("No arguments");
    }

    private static string GetArguments(LaunchArgumentsData launchArgumentsData, ArgumentsData arguments,
        string? mainClass, int launcherVersion)
    {
        var jvmArguments = BuildArguments(arguments.Jvm);
        var gameArguments = BuildArguments(arguments.Game);

        var jvmFilledArguments = FillJmvParameters(jvmArguments, launchArgumentsData, launcherVersion);
        var gameFilledArguments = FillGameArguments(gameArguments, launchArgumentsData);

        return $"{jvmFilledArguments} {mainClass} {gameFilledArguments}";
    }

    private static string? BuildArguments(IReadOnlyList<ArgumentItemData>? arguments)
    {
        if (arguments == null)
            return null;
        
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < arguments.Count; i++)
        {
            var argument = arguments[i];
            
            if (argument.Value == null)
                continue;
                
            if (!OsRuleManager.IsAllowed(argument.Rules))
                continue;
            
            for (var j = 0; j < argument.Value.Length; j++)
            {
                stringBuilder.Append(argument.Value[j]);
                
                if (j != argument.Value.Length - 1)
                    stringBuilder.Append(' ');
            }
            
            if (i != arguments.Count - 1)
                stringBuilder.Append(' ');
        }

        return stringBuilder.ToString();
    }

    private static string? FillJmvParameters(string? jvmArguments, LaunchArgumentsData launchArgumentsData,
        int launcherVersion)
    {
        if (string.IsNullOrEmpty(jvmArguments))
            return null;
        
        const char separator = ';';

        var classpathStringBuilder = new StringBuilder();
        classpathStringBuilder.Append('\"');
        for (var i = 0; i < launchArgumentsData.Libraries.Count; i++)
        {
            classpathStringBuilder.Append(launchArgumentsData.Libraries[i]);
            classpathStringBuilder.Append(separator);
        }
        classpathStringBuilder.Append(launchArgumentsData.ClientJar);
        classpathStringBuilder.Append('\"');

        return jvmArguments
            .Replace("${natives_directory}", launchArgumentsData.NativesDirectory)
            .Replace("${launcher_name}", "\"mclauncher\"")
            .Replace("${launcher_version}", launcherVersion.ToString())
            .Replace("${classpath}", classpathStringBuilder.ToString())
            .Replace("${classpath_separator}", separator.ToString())
            .Replace("${primary_jar}", launchArgumentsData.ClientJar)
            .Replace("${library_directory}", launchArgumentsData.LibrariesDirectory)
            .Replace("${game_directory}", launchArgumentsData.GameDirectory)
            .Replace("Windows 10", "\"Windows 10\"")
               + ' ' + launchArgumentsData.LoggingArgument.Replace("${path}", launchArgumentsData.LoggingFile);
    }

    private static string? FillGameArguments(string? gameArguments, LaunchArgumentsData launchArgumentsData)
    {
        if (string.IsNullOrEmpty(gameArguments))
            return null;
        
        string assetsVersion;
        var versionParts = launchArgumentsData.VersionId.Split('.');
        if (versionParts.Length == 3)
            assetsVersion = $"{versionParts[0]}.{versionParts[1]}";
        else
            assetsVersion = launchArgumentsData.VersionId;
        
        return gameArguments
            .Replace("${auth_player_name}", launchArgumentsData.PlayerName)
            .Replace("${version_name}", launchArgumentsData.VersionId)
            .Replace("${game_directory}", launchArgumentsData.GameDirectory)
            .Replace("${assets_root}", launchArgumentsData.AssetsDirectory)
            .Replace("${assets_index_name}", assetsVersion)
            .Replace("${auth_uuid}", GetUuid(launchArgumentsData.PlayerName))
            .Replace("${auth_access_token}", "null")
            .Replace("${clientid}", "null")
            .Replace("${auth_xuid}", "null")
            .Replace("${user_type}", "mojang")
            .Replace("${version_type}", launchArgumentsData.VersionType)
            .Replace("${resolution_width}", "1200")
            .Replace("${resolution_height}", "720")
            .Replace(" --demo", string.Empty);
    }
    
    private static string GetLegacyArguments(string legacyMinecraftArguments)
    {
        return "";
    }
    
    private static string GetUuid(string nickname)
    {
        var data = MD5.HashData(Encoding.Default.GetBytes(nickname));
        var guid = new Guid(data);
        return guid.ToString();
    }
}