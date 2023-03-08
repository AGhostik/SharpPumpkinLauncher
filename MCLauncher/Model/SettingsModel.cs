﻿using System.Threading.Tasks;
using MCLauncher.Model.Managers;
using MCLauncher.Model.VersionsJson;
using MCLauncher.UI;

namespace MCLauncher.Model;

public class SettingsModel : ISettingsModel
{
    private readonly IFileManager _fileManager;
    private readonly IJsonManager _jsonManager;
    private readonly IProfileManager _profileManager;

    public SettingsModel(IFileManager fileManager, IProfileManager profileManager, IJsonManager jsonManager)
    {
        _fileManager = fileManager;
        _profileManager = profileManager;
        _jsonManager = jsonManager;
    }

    public void SaveProfile(Profile? profile)
    {
        _profileManager.Save(profile);
        _profileManager.SaveLastProfileName(profile?.Name);
    }

    public Profile? LoadLastProfile()
    {
        return _profileManager.GetLast();
    }

    public void EditProfile(string? profileName, Profile? newProfile)
    {
        _profileManager.Edit(profileName, newProfile);
        _profileManager.SaveLastProfileName(newProfile?.Name);
    }

    public void OpenGameDirectory(string directory)
    {
        if (_fileManager.DirectoryExist(directory))
            _fileManager.StartProcess(directory);
    }

    public string FindJava()
    {
        return _fileManager.GetJavawPath();
    }

    public async Task<AllVersions> DownloadAllVersions()
    {
        var result = new AllVersions();

        var json = await _jsonManager.DownloadJsonAsync(ModelResource.VersionsUrl);
        var versions = json["versions"]?.ToObject<Version[]>();

        if (versions != null)
        {
            for (var i = 0; i < versions.Length; i++)
            {
                var version = versions[i];
                if (version.Id == null)
                    continue;

                if (version.Type == ModelResource.release)
                    result.Release.Add(version.Id);
                else if (version.Type == ModelResource.snapshot)
                    result.Snapshot.Add(version.Id);
                else if (version.Type == ModelResource.beta)
                    result.Beta.Add(version.Id);
                else if (version.Type == ModelResource.alpha)
                    result.Alpha.Add(version.Id);
            }
        }

        var latest = json["latest"]?.ToObject<Latest>();
        if (latest != null)
        {
            result.Latest = latest.Release;
            result.LatestSnapshot = latest.Snapshoot;
        }

        return result;
    }
}