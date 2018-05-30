using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Ionic.Zip;
using MCLauncher.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model
{
    public class FileManager
    {
        public JObject DownloadJson(string url)
        {
            string rawJson;
            using (var webClient = new WebClient())
            {
                rawJson = webClient.DownloadString(url);
            }

            return JObject.Parse(rawJson);
        }

        public TResult ParseJson<TResult>(string path)
        {
            JObject jObject;

            using (var streamReader = File.OpenText(path))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    jObject = (JObject) JToken.ReadFrom(jsonTextReader);
                }
            }

            return jObject.ToObject<TResult>();
        }

        public TResult ParseWebJson<TResult>(string url)
        {
            var jObject = DownloadJson(url);

            return jObject.ToObject<TResult>();
        }

        public void ExtractToDirectory(string sourceArchive, string destinationDirectory)
        {
            using (var zip = ZipFile.Read(sourceArchive))
            {
                foreach (var entry in zip)
                    entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            if (FileExist(path)) File.Delete(path);

            if (DirectoryExist(path)) Directory.Delete(path, true);
        }

        public bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public void StartProcess(string fileName)
        {
            Process.Start(fileName);
        }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public void SaveProfile(Profile profile)
        {
            if (Settings.Default.ProfileContainer == null)
                Settings.Default.ProfileContainer = new ProfileContainer();

            foreach (var profileFromContainer in Settings.Default.ProfileContainer.Profiles)
                if (profile.Name == profileFromContainer.Name)
                    return;
            Settings.Default.ProfileContainer.Profiles.Add(profile);

            Settings.Default.Save();
        }

        public List<Profile> GetProfiles()
        {
            if (Settings.Default.ProfileContainer == null)
                return new List<Profile>();

            return Settings.Default.ProfileContainer.Profiles;
        }

        public void EditProfile(string oldProfileName, Profile newProfile)
        {
            if (Settings.Default.ProfileContainer == null)
                throw new Exception("cant edit profile in null container");

            for (var i = 0; i < Settings.Default.ProfileContainer.Profiles.Count; i++)
            {
                if (Settings.Default.ProfileContainer.Profiles[i].Name != oldProfileName) continue;

                Settings.Default.ProfileContainer.Profiles[i] = newProfile;
                Settings.Default.Save();
                return;
            }
        }

        public Profile GetLastProfile()
        {
            if (Settings.Default.ProfileContainer == null || string.IsNullOrEmpty(Settings.Default.LastProfileName))
                return null;

            foreach (var profileFromContainer in Settings.Default.ProfileContainer.Profiles)
                if (profileFromContainer.Name == Settings.Default.LastProfileName)
                    return profileFromContainer;

            return null;
        }

        public void SaveLastProfileName(string name)
        {
            Settings.Default.LastProfileName = name;

            Settings.Default.Save();
        }

        public string GetLastProfileName()
        {
            if (string.IsNullOrEmpty(Settings.Default.LastProfileName))
                return string.Empty;

            return Settings.Default.LastProfileName;
        }
    }
}