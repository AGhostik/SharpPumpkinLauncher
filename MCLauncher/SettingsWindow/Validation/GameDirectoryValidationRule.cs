using System.Globalization;
using System.Windows.Controls;

namespace MCLauncher.SettingsWindow.Validation;

public sealed class GameDirectoryValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not string path)
            return new ValidationResult(false, "Value is not string");
        
        if (string.IsNullOrEmpty(path))
            return new ValidationResult(false, "Value is null or empty");
        
        return new ValidationResult(true, null);
        
        // var isPathFullyQualified = Path.IsPathFullyQualified(path);
        //
        // try
        // {
        //     var fullPath = Path.GetFullPath(path);
        //
        //     if (isPathFullyQualified)
        //     {
        //         if (fullPath == path)
        //             return new ValidationResult(true, null);
        //     }
        //     else
        //     {
        //         if (fullPath.EndsWith(path))
        //             return new ValidationResult(true, null);
        //     }
        //
        //     return new ValidationResult(false, null);
        // }
        // catch (SecurityException)
        // {
        //     return new ValidationResult(false, null);
        // }
        // catch (NotSupportedException)
        // {
        //     return new ValidationResult(false, null);
        // }
        // catch (PathTooLongException)
        // {
        //     return new ValidationResult(false, null);
        // }
    }
}