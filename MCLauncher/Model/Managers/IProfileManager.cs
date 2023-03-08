using System.Collections.Generic;

namespace MCLauncher.Model.Managers;

public interface IProfileManager
{
    void Delete(string? profileName);
    void Edit(string? profileName, Profile? newProfile);
    Profile? GetLast();
    string? GetLastProfileName();
    List<Profile?> GetProfiles();
    void Save(Profile? profile);
    void SaveLastProfileName(string? name);
}