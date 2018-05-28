using System.Diagnostics;
using System.IO;
using System.Net;
using System.IO.Compression;
using Ionic.Zip;
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

        public void ExtractToDirectory(string sourceArchive, string destinationDirectory)
        {
            using (var zip = ZipFile.Read(sourceArchive))
            { 
                foreach (var entry in zip)
                {
                    entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        public bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            if (FileExist(path))
            {
                File.Delete(path);
            }

            if (DirectoryExist(path))
            {
                Directory.Delete(path, true);
            }
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
    }
}