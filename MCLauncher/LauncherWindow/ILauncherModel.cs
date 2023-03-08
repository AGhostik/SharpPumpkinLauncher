using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCLauncher.LauncherWindow;

public interface ILauncherModel
{
    void DeleteProfile(string? name);
    string? GetLastProfile();
    List<string?> GetProfiles();
    void OpenNewProfileWindow();
    void OpenEditProfileWindow();
    void SaveLastProfileName(string? name);
    Task StartGame();
}