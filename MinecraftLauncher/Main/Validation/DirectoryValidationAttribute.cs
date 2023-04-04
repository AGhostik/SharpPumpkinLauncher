using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Main.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class DirectoryValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var result = DirectoryValidation.IsDirectoryValid(value as string, out var errorKey);
        ErrorMessage = errorKey;
        return result;
    }
}