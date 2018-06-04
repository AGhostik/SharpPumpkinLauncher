using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers
{
    public class JsonManager : IJsonManager
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

        public TResult ParseToObject<TResult>(string jsonFile)
        {
            JObject jObject;

            using (var streamReader = File.OpenText(jsonFile))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    jObject = (JObject) JToken.ReadFrom(jsonTextReader);
                }
            }

            return jObject.ToObject<TResult>();
        }

        public TResult DownloadToObject<TResult>(string url)
        {
            var jObject = DownloadJson(url);

            return jObject.ToObject<TResult>();
        }
    }
}