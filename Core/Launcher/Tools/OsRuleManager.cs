using JsonReader.Game;

namespace Launcher.Tools;

internal sealed class OsRuleManager
{
    private const string Allow = "allow";
    private const string Disallow = "disallow";
    
    private const string OsName = "windows";
    private const string OsVersion = "^10\\.";
    private const string OsArchitecture = "x64";
    
    public static bool IsAllowed(IReadOnlyList<RulesData>? rules)
    {
        if (rules == null)
            return true;
        
        for (var i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];
                        
            if (rule.Os == null)
                continue;
                        
            if ((rule.Os.Name == OsName || string.IsNullOrEmpty(rule.Os.Name)) && 
                (rule.Os.Version == OsVersion || string.IsNullOrEmpty(rule.Os.Version)) &&
                (rule.Os.Architecture == OsArchitecture || string.IsNullOrEmpty(rule.Os.Architecture)))
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