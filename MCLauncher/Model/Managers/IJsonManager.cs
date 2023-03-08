using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers
{
    public interface IJsonManager
    {
        JObject DownloadJson(string url);
        TResult DownloadToObject<TResult>(string url);
        TResult ParseToObject<TResult>(string jsonFile);
        Task<JObject> DownloadJsonAsync(string url);
        Task<TResult> DownloadToObjectAsync<TResult>(string url);
        Task<TResult> ParseToObjectAsync<TResult>(string jsonFile);
    }
}