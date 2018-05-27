using System.Collections.Generic;
using System.Linq;
using MCLauncher.Model.VersionsJson;
using MCLauncher.Properties;

namespace MCLauncher.Model
{
    public class SettingsModel
    {
        private readonly FileManager _fileManager;

        public SettingsModel(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void SaveLastProfile(Profile profile)
        {
            Settings.Default.LastProfile.Name = profile.Name;
            Settings.Default.LastProfile.Nickname = profile.Nickname;
            Settings.Default.LastProfile.JavaFile = profile.JavaFile;
            Settings.Default.LastProfile.GameDirectory = profile.GameDirectory;
            Settings.Default.LastProfile.JvmArgs = profile.JvmArgs;
            Settings.Default.LastProfile.LauncherVisibility = profile.LauncherVisibility;
            Settings.Default.LastProfile.MinecraftVersion = profile.MinecraftVersion;
            Settings.Default.LastProfile.ShowCustom = profile.ShowCustom;
            Settings.Default.LastProfile.ShowRelease = profile.ShowRelease;
            Settings.Default.LastProfile.ShowSnapshot = profile.ShowSnapshot;
            Settings.Default.LastProfile.ShowBeta = profile.ShowBeta;
            Settings.Default.LastProfile.ShowAlpha = profile.ShowAlpha;

            Settings.Default.Save();
        }

        public Profile LoadLastProfile()
        {
            if (Settings.Default.LastProfile == null)
                Settings.Default.LastProfile = new Profile();

            var profile = Settings.Default.LastProfile;

            return new Profile()
            {
                Name = profile.Name,
                Nickname = profile.Nickname,
                JavaFile = profile.JavaFile,
                GameDirectory = profile.GameDirectory,
                JvmArgs = profile.JvmArgs,
                LauncherVisibility = profile.LauncherVisibility,
                MinecraftVersion = profile.MinecraftVersion,
                ShowCustom = profile.ShowCustom,
                ShowRelease = profile.ShowRelease,
                ShowSnapshot = profile.ShowSnapshot,
                ShowBeta = profile.ShowBeta,
                ShowAlpha = profile.ShowAlpha
            };
        }

        public void OpenGameDirectory(Profile profile)
        {
            if (_fileManager.DirectoryExist(profile.GameDirectory))
                _fileManager.StartProcess(profile.GameDirectory);
        }

        public List<string> GetVersions(Profile profile)
        {
            var versions = new List<string>();

            var json = _fileManager.DownloadJson(ModelResource.Versions);
            var jVersions = json["versions"].ToObject<Version[]>();

            if (profile.ShowRelease)
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.release).Select(_ => _.Id));
            if (profile.ShowSnapshot)
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.snapshot).Select(_ => _.Id));
            if (profile.ShowBeta)
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.beta).Select(_ => _.Id));
            if (profile.ShowAlpha)
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.alpha).Select(_ => _.Id));

            return versions;
        }
    }
}