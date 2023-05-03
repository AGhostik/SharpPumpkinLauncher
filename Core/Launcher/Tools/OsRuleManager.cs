using JsonReader.PublicData.Game;
using JsonReader.PublicData.Runtime;
using SimpleLogger;

namespace Launcher.Tools;

internal sealed class OsRuleManager
{
    private const string Allow = "allow";
    private const string Disallow = "disallow";
    
    private const string OsWindows = "windows";
    private const string OsLinux = "linux";
    private const string OsOsx = "osx";
    private const string OsArchitecture64 = "x64";
    private const string OsArchitecture86 = "x84";

    private const string IsDemoUser = "is_demo_user";
    private const string HasCustomResolution = "has_custom_resolution";
    private const string HasQuickPlaysSupport = "has_quick_plays_support";
    private const string IsQuickPlaySingleplayer = "is_quick_play_singleplayer";
    private const string IsQuickPlayMultiplayer = "is_quick_play_multiplayer";
    private const string IsQuickPlayRealm = "is_quick_play_realms";

    private static readonly string _currentOsName;
    private static readonly string _currentOsVersion;
    private static readonly string _currentOsArchitecture;

    static OsRuleManager()
    {
        if (OperatingSystem.IsWindows())
            _currentOsName = OsWindows;
        else if (OperatingSystem.IsLinux())
            _currentOsName = OsLinux;
        else if (OperatingSystem.IsMacOS())
            _currentOsName = OsOsx;
        else
            _currentOsName = string.Empty;

        if (Environment.Is64BitOperatingSystem)
            _currentOsArchitecture = OsArchitecture64;
        else
            _currentOsArchitecture = OsArchitecture86;

        //osx version: "^10\\.5\\.\\d$"
        _currentOsVersion = $"^{Environment.OSVersion.Version.Major}\\.";
    }

    public static string CurrentOsName => _currentOsName;

    public static bool IsAllowed(IReadOnlyList<Rule>? rules, Features? features = null)
    {
        if (rules == null || rules.Count == 0)
            return true;

        var isAllowed = false;
        
        for (var i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];

            if (rule.Features != null)
            {
                foreach (var (featureName, value) in rule.Features)
                {
                    switch (featureName)
                    {
                        case IsDemoUser when value:
                            return false;
                        case HasCustomResolution when value:
                            return features?.UseCustomResolution ?? false;
                        case HasQuickPlaysSupport when value:
                            return false;
                        case IsQuickPlaySingleplayer when value:
                            return false;
                        case IsQuickPlayMultiplayer when value:
                            return false;
                        case IsQuickPlayRealm when value:
                            return false;
                        default:
                            Logger.Log($"Unknown feature '{featureName}'");
                            return false;
                    }
                }
            }

            if (rule.Os == null)
            {
                switch (rule.Action)
                {
                    case Allow:
                        isAllowed = true;
                        break;

                    case Disallow:
                        isAllowed = false;
                        break;
                }
                continue;
            }
            
            if (!string.IsNullOrEmpty(rule.Os.Name) && rule.Os.Name != _currentOsName)
                continue;

            if (!string.IsNullOrEmpty(rule.Os.Version) && rule.Os.Version != _currentOsVersion)
                continue;

            if (!string.IsNullOrEmpty(rule.Os.Architecture) && rule.Os.Architecture != _currentOsArchitecture)
                continue;

            switch (rule.Action)
            {
                case Allow:
                    return true;

                case Disallow:
                    return false;
            }
        }

        return isAllowed;
    }

    public static OsRuntime? GetOsRuntime(OsRuntimes osRuntimes)
    {
        switch (_currentOsName)
        {
            case OsWindows:
                switch (_currentOsArchitecture)
                {
                    case OsArchitecture64:
                        return osRuntimes.Windows64;
                    case OsArchitecture86:
                        return osRuntimes.Windows86;
                    default:
                        return null;
                }

            case OsOsx:
                return osRuntimes.MacOs;
            
            case OsLinux:
                return osRuntimes.Linux;
            
            default:
                return null;
        }
    }

    public static string? GetJavaExecutablePath()
    {
        switch (_currentOsName)
        {
            case OsWindows:
                return $"{OsWindows}\\bin\\javaw.exe";

            //todo: case OsOsx:
            //todo: case OsLinux:
            default:
                return null;
        }
    }
}