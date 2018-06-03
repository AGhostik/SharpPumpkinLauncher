using MCLauncher.UI;

namespace MCLauncher.Model
{
    public interface ISettingsModel
    {
        void EditProfile(string oldProfileName, Profile newProfile);
        AllVersions GetVersions();
        Profile LoadLastProfile();
        void OpenGameDirectory(string directory);
        string FindJava();
        void SaveProfile(Profile profile);
    }
}