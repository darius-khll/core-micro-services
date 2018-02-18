using System.Net.Http;
using System.Threading.Tasks;

namespace FirstService.Repository.Implementations
{
    public interface IHttpService
    {
        Task<string> GetStringAsync(string uri);
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
    }
}
