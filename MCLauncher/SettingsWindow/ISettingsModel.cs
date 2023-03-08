using System.Threading.Tasks;
using MCLauncher.Tools;

namespace MCLauncher.SettingsWindow;

public interface ISettingsModel
{
    void EditProfile(string? oldProfileName, Profile? newProfile);
    Task<AllVersions> DownloadAllVersions();
    Profile? LoadLastProfile();
    void OpenGameDirectory(string directory);
    string FindJava();
    void SaveProfile(Profile? profile);
}