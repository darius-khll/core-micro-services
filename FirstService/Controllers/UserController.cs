using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using System.Net.Http;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    public class UserController : Controller
    {

        private readonly IRedisClient _client;
        private readonly HttpClient _http;

        public UserController(IRedisClient client, HttpClient http)
        {
            _client = client;
            _http = http;
        }

        public async Task<string> Index()
        {
            _client.Set("foo", "bar123");
            string foo = _client.Get<string>("foo");

            var res = await _http.GetAsync("http://secondService");
            string content = await res.Content.ReadAsStringAsync();

            return string.Format($"First Service requested: {content} - {foo}");
        }
        
    }
}