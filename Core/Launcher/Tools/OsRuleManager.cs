using JsonReader.PublicData.Game;

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

    public static bool IsAllowed(IReadOnlyList<Rule>? rules)
    {
        if (rules == null)
            return true;
        
        for (var i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];

            if (rule.Features != null)
            {
                if (rule.Features.TryGetValue(IsDemoUser, out var value) && value)
                    return false;
            }

            if (rule.Os == null)
                continue;
            
            if ((rule.Os.Name == _currentOsName || string.IsNullOrEmpty(rule.Os.Name)) &&
                (rule.Os.Version == _currentOsVersion || string.IsNullOrEmpty(rule.Os.Version)) &&
                (rule.Os.Architecture == _currentOsArchitecture || string.IsNullOrEmpty(rule.Os.Architecture)))
            {
                switch (rule.Action)
                {
                    case Allow:
                        return true;

                    case Disallow:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}