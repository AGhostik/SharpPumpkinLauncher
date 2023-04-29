using System.IO;
using System.Linq;
using MinecraftLauncher.Properties;

namespace MinecraftLauncher.Main.Validation;

public static class DirectoryValidation
{
    public static bool IsDirectoryValid(string? path)
    {
        return IsDirectoryValid(path, out _);
    }
    
    public static bool IsDirectoryValid(string? path, out string errorKey)
    {
        if (string.IsNullOrEmpty(path))
        {
            errorKey = Localization.ValidationEmpty;
            return false;
        }

        var invalidPathChars = Path.GetInvalidPathChars();
        var previousPreviousChar = '\0';
        var previousChar = '\0';
        for (var i = 0; i < path.Length; i++)
        {
            var c = path[i];

            if (i == 2 && previousChar == ':' && char.IsLetter(previousPreviousChar))
            {
                if (c != '\\')
                {
                    errorKey = Localization.ValidationDirectoryInvalidPath;
                    return false;
                }
                
                var drivers = Directory.GetLogicalDrives();
                if (!drivers.Contains($"{char.ToUpper(previousPreviousChar)}:\\"))
                {
                    errorKey = Localization.ValidationDirectoryDriveNotExist;
                    return false;
                }
            }

            if (previousChar is '\\' or '/' and not '\0' && previousPreviousChar is '\\' or '/' and not '\0')
            {
                errorKey = Localization.ValidationDirectoryEmptyFolderInPath;
                return false;
            }

            if (invalidPathChars.Contains(c) || c is '?' or '<' or '>' or '\"' or '*' || (i != 1 && c == ':'))
            {
                errorKey = Localization.ValidationDirectoryRestrictedChar;
                return false;
            }

            previousPreviousChar = previousChar;
            previousChar = c;
        }
        
        errorKey = string.Empty;
        return true;
    }
}