using System.Globalization;
using System.Windows.Controls;

namespace MCLauncher.SettingsWindow.Validation;

public sealed class PlayerNameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not string stringValue)
            return new ValidationResult(false, "Value is not string");
        
        if (string.IsNullOrEmpty(stringValue))
            return new ValidationResult(false, "Value is null or empty");
        
        if (stringValue.Length < 3)
            return new ValidationResult(false, "Value is too short");
        
        if (stringValue.Length > 16)
            return new ValidationResult(false, "Value is too long");

        for (var i = 0; i < stringValue.Length; i++)
        {
            var c = stringValue[i];
            
            if (char.IsLetterOrDigit(c))
                continue;
            
            if (c == '_')
                continue;
            
            return new ValidationResult(false, $"Character '{c}' not allowed");
        }

        return new ValidationResult(true, null);
    }
}