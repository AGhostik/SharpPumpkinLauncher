namespace MinecraftLauncher.Main.Validation;

public static class PlayerNameValidation
{
    public static bool IsPlayerNameValid(string? playerName)
    {
        return IsPlayerNameValid(playerName, out _);
    }

    public static bool IsPlayerNameValid(string? playerName, out string errorKey)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            errorKey = "Empty";
            return false;
        }

        if (playerName.Length < 3)
        {
            errorKey = "Short";
            return false;
        }
        
        if (playerName.Length > 16)
        {
            errorKey = "Long";
            return false;
        }
        
        for (var i = 0; i < playerName.Length; i++)
        {
            var c = playerName[i];
            
            if (char.IsLetterOrDigit(c) || c == '_')
                continue;

            errorKey = "RestrictedChar";
            return false;
        }

        errorKey = string.Empty;
        return true;
    }
}