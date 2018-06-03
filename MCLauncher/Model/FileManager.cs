﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Ionic.Zip;
using MCLauncher.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model
{
    public class FileManager : IFileManager
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

        public void StartProcess(string fileName, string args, Action exitedAction)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(fileName, args),
                EnableRaisingEvents = true
            };

            process.Exited += (sender, eventArgs) =>
            {
                exitedAction.Invoke();
                Debug.WriteLine(process.ExitCode);
            };

            process.Start();
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

        public string GetPathFilename(string source)
        {
            return Path.GetFileName(source);
        }

        public string GetPathDirectory(string source)
        {
            return Path.GetDirectoryName(source);
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

        public void DeleteProfile(string name)
        {
            if (Settings.Default.ProfileContainer == null)
                return;

            foreach (var profileFromContainer in Settings.Default.ProfileContainer.Profiles)
            {
                if (profileFromContainer.Name != name) continue;

                Settings.Default.ProfileContainer.Profiles.Remove(profileFromContainer);
                break;
            }

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

        public string FindJava()
        {
            string java;

            using (var baseKey = Registry.LocalMachine.OpenSubKey(ModelResource.JavaKey))
            {
                if (baseKey == null) return string.Empty;

                var currentVersion = baseKey.GetValue("CurrentVersion").ToString();
                using (var homeKey = Registry.LocalMachine.OpenSubKey($"{ModelResource.JavaKey}\\{currentVersion}"))
                {
                    if (homeKey == null) return string.Empty;

                    java = $"{homeKey.GetValue("JavaHome")}\\bin\\javaw.exe";
                }
            }

            return java;
        }
    }
}