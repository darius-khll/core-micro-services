using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FirstService.Test
{
    public class IntegerationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public IntegerationTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .UseEnvironment("Development"));
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
            _client?.Dispose();
        }

        [Fact]
        public async Task ErrorPageShouldReturnSomething()
        {
            var response = await _client.GetAsync($"/api/home/error");

            //Remove EnsureSuccessStatusCode to see error in html
            //response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Error happened!", responseString);
        }
        
        [Fact]
        public async Task CacheController_RemoveCache_ShouldReturnDone()
        {
            var response = await _client.GetAsync($"/api/cache/RemoveCache");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("done", responseString);
        }

        [Fact]
        public async Task UserController_SetGetInRedis_ShouldReturnSomething()
        {
            var response = await _client.GetAsync($"/api/user/SetGetInRedis");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("bbb", responseString);
        }
    }
}
