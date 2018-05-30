﻿using System.Linq;
using MCLauncher.Model.VersionsJson;
using MCLauncher.UI;

namespace MCLauncher.Model
{
    public class SettingsModel
    {
        private readonly FileManager _fileManager;

        public SettingsModel(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void SaveProfile(Profile profile)
        {
            _fileManager.SaveProfile(profile);
        }

        public Profile LoadLastProfile()
        {
            return _fileManager.GetLastProfile();
        }

        public void EditProfile(string oldProfileName, Profile newProfile)
        {
            _fileManager.EditProfile(oldProfileName, newProfile);
        }

        public void OpenGameDirectory(string directory)
        {
            if (_fileManager.DirectoryExist(directory))
                _fileManager.StartProcess(directory);
        }

        public Versions GetVersions()
        {
            var versions = new Versions();

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