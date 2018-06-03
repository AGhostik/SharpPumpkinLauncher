using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model
{
    public interface IFileManager
    {
        void CreateDirectory(string directory);
        void Delete(string path);
        void DeleteProfile(string name);
        bool DirectoryExist(string path);
        JObject DownloadJson(string url);
        void EditProfile(string oldProfileName, Profile newProfile);
        void ExtractToDirectory(string sourceArchive, string destinationDirectory);
        bool FileExist(string path);
        string FindJava();
        Profile GetLastProfile();
        string GetLastProfileName();
        string GetPathDirectory(string source);
        string GetPathFilename(string source);
        List<Profile> GetProfiles();
        TResult ParseJson<TResult>(string path);
        TResult ParseWebJson<TResult>(string url);
        void SaveLastProfileName(string name);
        void SaveProfile(Profile profile);
        void StartProcess(string fileName);
    }
}