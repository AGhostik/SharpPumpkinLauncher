using MCLauncher.LauncherWindow;

namespace MCLauncher.Messages;

public class ShowSettingsMessage
{
    public ShowSettingsMessage(ProfileViewModel? profile)
    {
        Profile = profile;
    }

    public ProfileViewModel? Profile { get; }
}