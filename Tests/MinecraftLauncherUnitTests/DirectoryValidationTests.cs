using MinecraftLauncher.Main.Validation;

namespace MinecraftLauncherUnitTests;

public class DirectoryValidationTests
{
    [Test]
    public void DirectoryValidation_True_RelativePath()
    {
        var result = DirectoryValidation.IsDirectoryValid("Minecraft");
        Assert.IsTrue(result);
    }
    
    [Test]
    public void DirectoryValidation_True_RelativePath_ContainBackSlashes()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft\\");
        Assert.IsTrue(result);
    }
    
    [Test]
    public void DirectoryValidation_True_AbsolutePath_Short()
    {
        var result = DirectoryValidation.IsDirectoryValid("C:\\SomeFolder");
        Assert.IsTrue(result);
    }
    
    [Test]
    public void DirectoryValidation_True_AbsolutePath_DriveLetterIsUpper()
    {
        var result = DirectoryValidation.IsDirectoryValid("C:\\SomeFolder\\Minecraft");
        Assert.IsTrue(result);
    }
    
    [Test]
    public void DirectoryValidation_True_AbsolutePath_DriveLetterIsLower()
    {
        var result = DirectoryValidation.IsDirectoryValid("c:\\SomeFolder\\Minecraft");
        Assert.IsTrue(result);
    }
    
    [Test]
    public void DirectoryValidation_True_AbsolutePath_ContainWhitespaces()
    {
        var result = DirectoryValidation.IsDirectoryValid("C:\\Some Folder\\Mine craft");
        Assert.IsTrue(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathIsEmptyString()
    {
        var result = DirectoryValidation.IsDirectoryValid(string.Empty);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathIsNull()
    {
        var result = DirectoryValidation.IsDirectoryValid(null);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_IsFileRelativePath()
    {
        var result = DirectoryValidation.IsDirectoryValid("Minecraft\\Text.txt");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_AbsolutePathMissingBackSlash()
    {
        var result = DirectoryValidation.IsDirectoryValid("C:Minecraft");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_IsFileAbsolutePath()
    {
        var result = DirectoryValidation.IsDirectoryValid("C:\\SomeFolder\\Minecraft\\Text.txt");
        Assert.IsFalse(result);
    }

    [Test]
    public void DirectoryValidation_False_AbsolutePath_MissingDriveLetter()
    {
        var result = DirectoryValidation.IsDirectoryValid(":\\SomeFolder\\Minecraft");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_AbsolutePath_DriveNotExist()
    {
        var result = DirectoryValidation.IsDirectoryValid("X:\\SomeFolder\\Minecraft");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_Pipe()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft|");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_GreaterThan()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft>");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_LessThan()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft<");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_SpeechMark()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft\"");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_QuestionMark()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft?");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_Asterix()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft*");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_Colon()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\Minecraft:");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_BackSlashAfterBackSlash()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\\\Minecraft");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_ForwardSlashAfterBackSlash()
    {
        var result = DirectoryValidation.IsDirectoryValid("\\/Minecraft");
        Assert.IsFalse(result);
    }
    
    [Test]
    public void DirectoryValidation_False_PathContain_BackSlashAfterForwardSlash()
    {
        var result = DirectoryValidation.IsDirectoryValid("/\\Minecraft");
        Assert.IsFalse(result);
    }
}