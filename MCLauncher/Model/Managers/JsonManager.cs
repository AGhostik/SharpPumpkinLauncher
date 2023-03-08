using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers
{
    public sealed class JsonManager : IJsonManager
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
        
        public async Task<JObject> DownloadJsonAsync(string url)
        {
            using var webClient = new WebClient();
            var rawJson = await webClient.DownloadStringTaskAsync(url);
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
        
        public async Task<TResult> ParseToObjectAsync<TResult>(string jsonFile)
        {
            using var streamReader = File.OpenText(jsonFile);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jObject = await JToken.ReadFromAsync(jsonTextReader);
            return jObject.ToObject<TResult>();
        }

        public TResult DownloadToObject<TResult>(string url)
        {
            var jObject = DownloadJson(url);

            return jObject.ToObject<TResult>();
        }
        
        public async Task<TResult> DownloadToObjectAsync<TResult>(string url)
        {
            var jObject = await DownloadJsonAsync(url);
            return jObject.ToObject<TResult>();
        }
    }
}