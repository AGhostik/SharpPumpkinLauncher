namespace MCLauncher.Messages;

public class ShowSettingsMessage
{
    public ShowSettingsMessage(bool isNewProfile)
    {
        IsNewProfile = isNewProfile;
    }
    
    public bool IsNewProfile { get; }
}