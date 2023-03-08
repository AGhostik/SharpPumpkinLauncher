using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers;

public sealed class JsonManager : IJsonManager
{
    public async Task<JObject> DownloadJsonAsync(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(new Uri(url));
        var json = await response.Content.ReadAsStringAsync();
        return JObject.Parse(json);
    }
        
    public async Task<TResult?> ParseToObjectAsync<TResult>(string jsonFile)
    {
        using var streamReader = File.OpenText(jsonFile);
        await using var jsonTextReader = new JsonTextReader(streamReader);
        var jObject = await JToken.ReadFromAsync(jsonTextReader);
        return jObject.ToObject<TResult>();
    }
}