using System.Text;
using JsonReader.PublicData.Game;

namespace Launcher.Tools;

internal sealed class LaunchArgumentsBuilder
{
    public static string GetLaunchArguments(MinecraftData minecraftVersionData, LaunchArgumentsData launchArgumentsData)
    {
        if (minecraftVersionData.Arguments.LegacyArguments == null)
        {
            return GetArguments(launchArgumentsData, minecraftVersionData.Arguments, minecraftVersionData.MainClass,
                minecraftVersionData.MinimumLauncherVersion);
        }

        return GetLegacyArguments(launchArgumentsData, minecraftVersionData.Arguments.LegacyArguments,
            minecraftVersionData.MainClass, minecraftVersionData.MinimumLauncherVersion);
    }

    private static string GetArguments(LaunchArgumentsData launchArgumentsData, Arguments arguments,
        string mainClass, int launcherVersion)
    {
        var jvmArguments = BuildArguments(arguments.Jvm);
        var gameArguments = BuildArguments(arguments.Game);

        var jvmFilledArguments = FillJmvParameters(jvmArguments, launchArgumentsData, launcherVersion);
        var gameFilledArguments = FillGameArguments(gameArguments, launchArgumentsData);

        return $"{jvmFilledArguments} {mainClass} {gameFilledArguments}";
    }

    private static string? BuildArguments(IReadOnlyList<ArgumentItem>? arguments)
    {
        if (arguments == null)
            return null;
        
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < arguments.Count; i++)
        {
            var argument = arguments[i];

            if (!OsRuleManager.IsAllowed(argument.Rules))
                continue;
            
            for (var j = 0; j < argument.Values.Count; j++)
            {
                stringBuilder.Append(argument.Values[j]);
                
                if (j != argument.Values.Count - 1)
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

        var classpathString =
            BuildLibrariesString(launchArgumentsData.Libraries, launchArgumentsData.ClientJar, separator);

        var result = jvmArguments
            .Replace("${natives_directory}", launchArgumentsData.NativesDirectory)
            .Replace("${launcher_name}", launchArgumentsData.LauncherName)
            .Replace("${launcher_version}", launcherVersion.ToString())
            .Replace("${classpath}", classpathString)
            .Replace("${classpath_separator}", separator.ToString())
            .Replace("${primary_jar}", launchArgumentsData.ClientJar)
            .Replace("${library_directory}", launchArgumentsData.LibrariesDirectory)
            .Replace("${game_directory}", launchArgumentsData.GameDirectory);
        
        if (!string.IsNullOrEmpty(launchArgumentsData.LoggingArgument) &&
            !string.IsNullOrEmpty(launchArgumentsData.LoggingFile))
        {
            result += ' ' + launchArgumentsData.LoggingArgument.Replace("${path}", launchArgumentsData.LoggingFile);
        }

        return result;
    }

    private static string BuildLibrariesString(IReadOnlyList<string> libraries, string clientJar, char separator)
    {
        var classpathStringBuilder = new StringBuilder();
        classpathStringBuilder.Append('\"');
        for (var i = 0; i < libraries.Count; i++)
        {
            classpathStringBuilder.Append(libraries[i]);
            classpathStringBuilder.Append(separator);
        }
        classpathStringBuilder.Append(clientJar);
        classpathStringBuilder.Append('\"');

        return classpathStringBuilder.ToString();
    }

    private static string? FillGameArguments(string? gameArguments, LaunchArgumentsData launchArgumentsData)
    {
        if (string.IsNullOrEmpty(gameArguments))
            return null;

        return gameArguments
            .Replace("${auth_player_name}", launchArgumentsData.PlayerName)
            .Replace("${version_name}", launchArgumentsData.VersionId)
            .Replace("${game_directory}", launchArgumentsData.GameDirectory)
            .Replace("${assets_root}", launchArgumentsData.AssetsDirectory)
            .Replace("${assets_index_name}", launchArgumentsData.AssetsVersion)
            .Replace("${auth_uuid}", launchArgumentsData.AuthUuid)
            .Replace("${auth_access_token}", launchArgumentsData.AuthAccessToken)
            .Replace("${clientid}", launchArgumentsData.ClientId)
            .Replace("${auth_xuid}", launchArgumentsData.AuthXuid)
            .Replace("${user_type}", launchArgumentsData.UserType)
            .Replace("${version_type}", launchArgumentsData.VersionType)
            .Replace("${resolution_width}", launchArgumentsData.Width)
            .Replace("${resolution_height}", launchArgumentsData.Height);
        //  .Replace("${user_properties}", "{}");
        //  .Replace("${auth_session}", "null");
    }
    
    private static string GetLegacyArguments(LaunchArgumentsData launchArgumentsData, LegacyArguments legacyArguments,
        string mainClass, int launcherVersion)
    {
        var jvmFilledArguments = FillJmvParameters(legacyArguments.Jvm, launchArgumentsData, launcherVersion);
        var gameFilledArguments = FillGameArguments(legacyArguments.Game, launchArgumentsData);
        
        return $"{jvmFilledArguments} {mainClass} {gameFilledArguments}";
    }
}