using Newtonsoft.Json.Linq;

namespace MCLauncher.Model.Managers
{
    public interface IJsonManager
    {
        JObject DownloadJson(string url);
        TResult DownloadToObject<TResult>(string url);
        TResult ParseToObject<TResult>(string jsonFile);
    }
}