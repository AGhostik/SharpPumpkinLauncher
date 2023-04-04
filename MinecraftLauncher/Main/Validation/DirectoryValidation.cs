using System;
using System.IO;
using System.Linq;
using MinecraftLauncher.Resources;

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

            if (i == 2 && c == '\\' && previousChar == ':' && char.IsLetter(previousPreviousChar))
            {
                //is absolute path
                var drivers = Directory.GetLogicalDrives();
                if (!drivers.Contains($"{previousPreviousChar}:\\"))
                {
                    errorKey = "Drive not exist";
                    return false;
                }
            }

            if (previousChar is '\\' or '/' and not '\0' && previousPreviousChar is '\\' or '/' and not '\0')
            {
                errorKey = "Empte directory in path";
                return false;
            }

            if (invalidPathChars.Contains(c) || c is '?' or '<' or '>' or '\"' or '*' or '.' || (i != 1 && c == ':'))
            {
                errorKey = "Invalid character";
                return false;
            }

            previousPreviousChar = previousChar;
            previousChar = c;
        }
        
        errorKey = string.Empty;
        return true;
    }
}