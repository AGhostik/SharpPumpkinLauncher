using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCLauncher.Model
{
    public interface IMainModel
    {
        void DeleteProfile(string name);
        string GetLastProfile();
        List<string> GetProfiles();
        void OpenProfileCreatingWindow();
        void OpenProfileEditingWindow();
        void SaveLastProfileName(string name);
        Task StartGame();
    }
}