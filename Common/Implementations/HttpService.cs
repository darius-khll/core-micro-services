using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Implementations
{
    public interface IHttpService
    {
        Task<string> GetStringAsync(string uri);
        Task<string> PostAsync(string uri, object jsonObject);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStringAsync(string uri)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> PostAsync(string uri, object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            var strContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(uri, strContent);
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
