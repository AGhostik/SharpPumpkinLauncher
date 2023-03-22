using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using UserSettings;

namespace MCLauncher.SettingsWindow.Validation;

public sealed class ProfileNameValidationRule : ValidationRule
{
    private readonly List<string> _profileNames = new();
    
    public ProfileNameValidationRule()
    {
        var profiles = LauncherSettings.Instance.Data.Profiles;
        if (profiles != null)
        {
            for (var i = 0; i < profiles.Count; i++)
            {
                var profileName = profiles[i].Name;
                if (!string.IsNullOrEmpty(profileName))
                    _profileNames.Add(profileName);
            }
        }
    }
    
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not string stringValue)
            return new ValidationResult(false, "Value is not string");
        
        if (string.IsNullOrEmpty(stringValue))
            return new ValidationResult(false, "Value is null or empty");

        if (_profileNames.Contains(stringValue))
            return new ValidationResult(false, "Same profile name exist");

        return new ValidationResult(true, null);
    }
}