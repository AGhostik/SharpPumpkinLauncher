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
        
        errorKey = string.Empty;
        return true;
    }
}