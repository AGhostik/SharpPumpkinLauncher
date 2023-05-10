using SharpPumpkinLauncher.Main.Validation;

namespace MinecraftLauncherUnitTests;

public class PlayerNameValidationTests
{
    [Test]
    public void PlayerNameValidation_True_NameValid()
    {
        const string shortPlayerName = "AaAaAaAaAaAaAaAa";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsTrue(result);
    }
    
    [Test]
    public void PlayerNameValidation_True_NameValid_ContainUnderscore()
    {
        const string shortPlayerName = "_AaAa_";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsTrue(result);
    }

    [Test]
    public void PlayerNameValidation_False_NameTooShort()
    {
        const string shortPlayerName = "Aa";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameTooLong()
    {
        const string shortPlayerName = "AaAaAaAaAaAaAaAaA";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameIsEmptyString()
    {
        var result = PlayerNameValidation.IsPlayerNameValid(string.Empty);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameIsNull()
    {
        var result = PlayerNameValidation.IsPlayerNameValid(null);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_ExcamlationMark()
    {
        const string shortPlayerName = "Aaa!";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_AtSign()
    {
        const string shortPlayerName = "Aaa@";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Actue()
    {
        const string shortPlayerName = "Aaa\'";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_SpeechMark()
    {
        const string shortPlayerName = "Aaa\"";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Hash()
    {
        const string shortPlayerName = "Aaa#";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_NumeroSign()
    {
        const string shortPlayerName = "Aaa№";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_DollarSign()
    {
        const string shortPlayerName = "Aaa$";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }

    [Test]
    public void PlayerNameValidation_False_NameContain_SemiColon()
    {
        const string shortPlayerName = "Aaa;";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_PercentageSign()
    {
        const string shortPlayerName = "Aaa%";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Caret()
    {
        const string shortPlayerName = "Aaa^";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Colon()
    {
        const string shortPlayerName = "Aaa:";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_AndSign()
    {
        const string shortPlayerName = "Aaa&";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_QuestionMark()
    {
        const string shortPlayerName = "Aaa?";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Asterix()
    {
        const string shortPlayerName = "Aaa*";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_OpenBracket()
    {
        const string shortPlayerName = "Aaa(";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_CloseBracket()
    {
        const string shortPlayerName = "Aaa)";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Hyphen()
    {
        const string shortPlayerName = "Aaa-";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Plus()
    {
        const string shortPlayerName = "Aaa+";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_EqualSign()
    {
        const string shortPlayerName = "Aaa=";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Pipe()
    {
        const string shortPlayerName = "Aaa|";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_BackSlash()
    {
        const string shortPlayerName = "Aaa\\";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_ForwardSlash()
    {
        const string shortPlayerName = "Aaa/";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Apostrophe()
    {
        const string shortPlayerName = "Aaa`";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Tide()
    {
        const string shortPlayerName = "Aaa~";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_LessThan()
    {
        const string shortPlayerName = "Aaa<";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_GreaterThan()
    {
        const string shortPlayerName = "Aaa>";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_FullStop()
    {
        const string shortPlayerName = "Aaa.";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void PlayerNameValidation_False_NameContain_Comma()
    {
        const string shortPlayerName = "Aaa,";
        var result = PlayerNameValidation.IsPlayerNameValid(shortPlayerName);
        Assert.IsFalse(result);
    }
}