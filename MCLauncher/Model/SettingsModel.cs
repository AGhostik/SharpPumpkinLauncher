using System.Linq;
using MCLauncher.Model.Managers;
using MCLauncher.Model.VersionsJson;
using MCLauncher.UI;

namespace MCLauncher.Model
{
    public class SettingsModel : ISettingsModel
    {
        private readonly IFileManager _fileManager;

        public SettingsModel(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void SaveProfile(Profile profile)
        {
            _fileManager.SaveProfile(profile);
            _fileManager.SaveLastProfileName(profile.Name);
        }

        public Profile LoadLastProfile()
        {
            return _fileManager.GetLastProfile() ?? new Profile();
        }

        public void EditProfile(string oldProfileName, Profile newProfile)
        {
            _fileManager.EditProfile(oldProfileName, newProfile);
            _fileManager.SaveLastProfileName(newProfile.Name);
        }

        public void OpenGameDirectory(string directory)
        {
            if (_fileManager.DirectoryExist(directory))
                _fileManager.StartProcess(directory);
        }

        public string FindJava()
        {
            return _fileManager.FindJava();
        }

        public AllVersions GetVersions()
        {
            var versions = new AllVersions();

            var json = _fileManager.DownloadJson(ModelResource.VersionsUrl);
            var jVersions = json["versions"].ToObject<Version[]>();

            versions.Release.AddRange(jVersions.Where(_ => _.Type == ModelResource.release).Select(_ => _.Id));
            versions.Snapshot.AddRange(jVersions.Where(_ => _.Type == ModelResource.snapshot).Select(_ => _.Id));
            versions.Beta.AddRange(jVersions.Where(_ => _.Type == ModelResource.beta).Select(_ => _.Id));
            versions.Alpha.AddRange(jVersions.Where(_ => _.Type == ModelResource.alpha).Select(_ => _.Id));

            return versions;
        }
    }
}