using SharpPumpkinLauncher.Main.Validation;

namespace MinecraftLauncherUnitTests;

public class ProfileNameValidationTests
{
    [Test]
    public void ProfileNameValidation_True_NameValid()
    {
        var result = ProfileNameValidation.IsProfileNameValid("Name", null);
        Assert.IsTrue(result);
    }
    
    [Test]
    public void ProfileNameValidation_False_NameIsEmptyString()
    {
        var result = ProfileNameValidation.IsProfileNameValid(string.Empty, null);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ProfileNameValidation_False_NameIsNull()
    {
        var result = ProfileNameValidation.IsProfileNameValid(null, null);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ProfileNameValidation_False_NameIsRestricted()
    {
        var result = ProfileNameValidation.IsProfileNameValid("Name", new List<string?>() { "Name" });
        Assert.IsFalse(result);
    }
}