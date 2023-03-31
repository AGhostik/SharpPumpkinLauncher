using System.Collections.Generic;

namespace MinecraftLauncher.Main.Validation;

public static class ProfileNameValidation
{
    public static bool IsProfileNameValid(string? profileName, ICollection<string?>? restrictedNames)
    {
        return IsProfileNameValid(profileName, restrictedNames, out _);
    }
    
    public static bool IsProfileNameValid(string? profileName, ICollection<string?>? restrictedNames, out string errorKey)
    {
        if (string.IsNullOrEmpty(profileName))
        {
            errorKey = "Empty";
            return false;
        }

        if (restrictedNames != null && restrictedNames.Contains(profileName))
        {
            errorKey = "Restricted";
            return false;
        }

        errorKey = string.Empty;
        return true;
    }
}