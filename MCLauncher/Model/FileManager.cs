using System.Diagnostics;
using System.IO;
using System.Net;
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

        public bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public void StartProcess(string fileName)
        {
            Process.Start(fileName);
        }
    }
}