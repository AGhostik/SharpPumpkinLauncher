using System.Threading.Tasks;
using MCLauncher.Json;
using MCLauncher.Json.VersionsJson;
using MCLauncher.Properties;
using MCLauncher.Tools;
using MCLauncher.Tools.Interfaces;

namespace MCLauncher.SettingsWindow;

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

                if (version.Type == WellKnownValues.Release)
                    result.Release.Add(version.Id);
                else if (version.Type == WellKnownValues.Snapshot)
                    result.Snapshot.Add(version.Id);
                else if (version.Type == WellKnownValues.Beta)
                    result.Beta.Add(version.Id);
                else if (version.Type == WellKnownValues.Alpha)
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