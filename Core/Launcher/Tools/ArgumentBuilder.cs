using System.Text;
using JsonReader.PublicData.Game;
using SimpleLogger;

namespace Launcher.Tools;

internal static class ArgumentBuilder
{
    public static string? BuildArguments(IReadOnlyList<string>? arguments, string? firstPart = null)
    {
        if (arguments == null)
            return null;
        
        var stringBuilder = new StringBuilder();

        if (!string.IsNullOrEmpty(firstPart))
        {
            stringBuilder.Append(firstPart);
            stringBuilder.Append(' ');
        }

        for (var i = 0; i < arguments.Count; i++)
        {
            stringBuilder.Append(arguments[i]);
            stringBuilder.Append(' ');
        }

        return stringBuilder.ToString();
    }
    
    public static string? BuildArguments(IReadOnlyList<ArgumentItem>? arguments, Features? features = default)
    {
        if (arguments == null)
            return null;
        
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < arguments.Count; i++)
        {
            var argument = arguments[i];

            if (!OsRuleManager.IsAllowed(argument.Rules, features))
                continue;
            
            for (var j = 0; j < argument.Values.Count; j++)
            {
                var value = argument.Values[j];
                if (value.Contains(' '))
                    stringBuilder.Append(EscapeWhitespaces(value));
                else
                    stringBuilder.Append(value);

                if (j != argument.Values.Count - 1)
                    stringBuilder.Append(' ');
            }
            
            if (i != arguments.Count - 1)
                stringBuilder.Append(' ');
        }

        return stringBuilder.ToString();
    }
    
    public static string? FillGameArguments(string? gameArguments, LaunchArgumentsData launchArgumentsData)
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
            .Replace("${resolution_height}", launchArgumentsData.Height)
            .Replace("${game_assets}", launchArgumentsData.AssetsDirectory)
            .Replace("${user_properties}", "{}")
            .Replace("${auth_session}", "null");
    }
    
    public static string? FillJmvParameters(string? jvmArguments, LaunchArgumentsData launchArgumentsData,
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
            .Replace("${game_directory}", launchArgumentsData.GameDirectory)
            .Replace("${version_name}", launchArgumentsData.VersionId);
        
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

    private static string EscapeWhitespaces(string value)
    {
        if (value.Contains('='))
        {
            var valueParts = value.Split('=');
            if (valueParts.Length < 2)
            {
                Logger.Log($"Unknown argument: '{value}'");
                return $"\"{value}\"";
            }

            var sb = new StringBuilder();
            for (var i = 0; i < valueParts.Length; i++)
            {
                var part = valueParts[i];
                if (part.Contains(' '))
                {
                    sb.Append('\"');
                    sb.Append(part);
                    sb.Append('\"');
                }
                else
                {
                    sb.Append(part);
                }

                if (i != valueParts.Length - 1)
                    sb.Append('=');
            }

            return sb.ToString();
        }
        
        return $"\"{value}\"";
    }
}