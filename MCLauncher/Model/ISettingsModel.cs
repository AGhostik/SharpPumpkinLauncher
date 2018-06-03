using MCLauncher.UI;

namespace MCLauncher.Model
{
    public interface ISettingsModel
    {
        void EditProfile(string oldProfileName, Profile newProfile);
        Versions GetVersions();
        Profile LoadLastProfile();
        void OpenGameDirectory(string directory);
        void SaveProfile(Profile profile);
    }
}