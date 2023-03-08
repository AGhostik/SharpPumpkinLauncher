using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers
{
    public interface IJsonManager
    {
        Task<JObject> DownloadJsonAsync(string url);
        Task<TResult> ParseToObjectAsync<TResult>(string jsonFile);
    }
}