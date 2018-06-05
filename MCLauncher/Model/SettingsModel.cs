using System.Linq;
using MCLauncher.Model.Managers;
using MCLauncher.Model.VersionsJson;
using MCLauncher.UI;

namespace MCLauncher.Model
{
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

        public void SaveProfile(Profile profile)
        {
            _profileManager.Save(profile);
            _profileManager.SaveLastProfileName(profile.Name);
        }

        public Profile LoadLastProfile()
        {
            return _profileManager.GetLast() ?? new Profile();
        }

        public void EditProfile(string profileName, Profile newProfile)
        {
            _profileManager.Edit(profileName, newProfile);
            _profileManager.SaveLastProfileName(newProfile.Name);
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

        public AllVersions DownloadAllVersions()
        {
            var versions = new AllVersions();

            var json = _jsonManager.DownloadJson(ModelResource.VersionsUrl);
            var jVersions = json["versions"].ToObject<Version[]>();

            versions.Release.AddRange(jVersions.Where(_ => _.Type == ModelResource.release).Select(_ => _.Id));
            versions.Snapshot.AddRange(jVersions.Where(_ => _.Type == ModelResource.snapshot).Select(_ => _.Id));
            versions.Beta.AddRange(jVersions.Where(_ => _.Type == ModelResource.beta).Select(_ => _.Id));
            versions.Alpha.AddRange(jVersions.Where(_ => _.Type == ModelResource.alpha).Select(_ => _.Id));

            return versions;
        }
    }
}