using UserSettings;

namespace MCLauncher.Messages;

public sealed class ProfileSaved
{
    public ProfileSaved(ProfileData profileData)
    {
        ProfileData = profileData;
    }
    
    public ProfileData ProfileData { get; }
}