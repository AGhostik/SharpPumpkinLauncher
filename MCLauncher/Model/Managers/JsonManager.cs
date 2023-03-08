using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers
{
    public sealed class JsonManager : IJsonManager
    {
        
        public async Task<JObject> DownloadJsonAsync(string url)
        {
            using var webClient = new WebClient();
            var rawJson = await webClient.DownloadStringTaskAsync(url);
            return JObject.Parse(rawJson);
        }
        
        public async Task<TResult> ParseToObjectAsync<TResult>(string jsonFile)
        {
            using var streamReader = File.OpenText(jsonFile);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jObject = await JToken.ReadFromAsync(jsonTextReader);
            return jObject.ToObject<TResult>();
        }
    }
}