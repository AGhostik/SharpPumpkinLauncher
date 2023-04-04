using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Main.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class PlayerNameValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var result = PlayerNameValidation.IsPlayerNameValid(value as string, out var errorKey);
        ErrorMessage = errorKey;
        return result;
    }
}