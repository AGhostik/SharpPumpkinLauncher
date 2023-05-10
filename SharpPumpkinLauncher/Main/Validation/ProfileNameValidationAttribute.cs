using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpPumpkinLauncher.Main.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class ProfileNameValidationAttribute : ValidationAttribute
{
    public static ICollection<string?>? RestrictedName { get; set; }

    public override bool IsValid(object? value)
    {
        var result = ProfileNameValidation.IsProfileNameValid(value as string, RestrictedName, out var errorKey);
        ErrorMessage = errorKey;
        return result;
    }
}