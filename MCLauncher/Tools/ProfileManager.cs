using System.Collections.Generic;
using MCLauncher.Properties;
using MCLauncher.Tools.Interfaces;

namespace MCLauncher.Tools;

public class ProfileManager : IProfileManager
{
    public void Save(Profile? profile)
    {
        if (Settings.Default.ProfileContainer == null)
            Settings.Default.ProfileContainer = new ProfileContainer();

        foreach (var profileFromContainer in Settings.Default.ProfileContainer.Profiles)
        {
            if (profile == null || profileFromContainer == null)
                continue;
            
            if (profile.Name == profileFromContainer.Name)
                return;
        }

        Settings.Default.ProfileContainer.Profiles.Add(profile);
        Settings.Default.Save();
    }

    public void Delete(string? profileName)
    {
        if (Settings.Default.ProfileContainer == null)
            return;

        foreach (var profileFromContainer in Settings.Default.ProfileContainer.Profiles)
        {
            if (profileFromContainer?.Name != profileName)
                continue;

            Settings.Default.ProfileContainer.Profiles.Remove(profileFromContainer);
            break;
        }

        Settings.Default.Save();
    }

    public List<Profile?> GetProfiles()
    {
        return Settings.Default.ProfileContainer == null
            ? new List<Profile?>()
            : Settings.Default.ProfileContainer.Profiles;
    }

    public void Edit(string? profileName, Profile? newProfile)
    {
        for (var i = 0; i < Settings.Default.ProfileContainer.Profiles.Count; i++)
        {
            if (Settings.Default.ProfileContainer.Profiles[i]?.Name != profileName)
                continue;

            Settings.Default.ProfileContainer.Profiles[i] = newProfile;
            Settings.Default.Save();
            return;
        }
    }

    public Profile? GetLast()
    {
        if (Settings.Default.ProfileContainer == null || string.IsNullOrEmpty(Settings.Default.LastProfileName))
            return null;

        foreach (var profileFromContainer in Settings.Default.ProfileContainer.Profiles)
        {
            if (profileFromContainer?.Name == Settings.Default.LastProfileName)
                return profileFromContainer;
        }

        return null;
    }

    public void SaveLastProfileName(string? name)
    {
        Settings.Default.LastProfileName = name;
        Settings.Default.Save();
    }

    public string? GetLastProfileName()
    {
        if (string.IsNullOrEmpty(Settings.Default.LastProfileName))
            return string.Empty;

        return Settings.Default.LastProfileName;
    }
}