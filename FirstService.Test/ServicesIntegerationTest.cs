using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FirstService.Test
{
    public class ServicesIntegerationTest
    {
        //private readonly TestServer _server;
        private readonly HttpClient _client;
        const string FirstService = "http://localhost:8181/api";

        public ServicesIntegerationTest()
        {
            //_server = new TestServer(WebHost.CreateDefaultBuilder()
            //    .UseStartup<TestStartup>()
            //    .UseEnvironment("Development"));
            //_client = _server.CreateClient();
            _client = new HttpClient();
        }

        public void Dispose()
        {
            //_server.Dispose();
            _client?.Dispose();
        }

        [Fact]
        public async Task ErrorPageShouldReturnSomething()
        {
            var response = await _client.GetAsync($"{FirstService}/home/error");

            //Remove EnsureSuccessStatusCode to see error in html
            //response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Error happened!", responseString);
        }

        [Fact]
        public async Task UserController_SetGetInRedis_ShouldReturnSomething()
        {
            var response = await _client.GetAsync($"{FirstService}/user/SetGetInRedis");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("bbb", responseString);
        }
        

        [Fact]
        public async Task CacheController_RemoveCache_ShouldReturnDone()
        {
            var response = await _client.GetAsync($"{FirstService}/cache/RemoveCache");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("done", responseString);
        }

        [Fact]
        public async Task AddToServiceBusController_AddToServiceBus_ShouldReturnCorrectValue()
        {
            var response = await _client.GetAsync($"{FirstService}/servicebus/AddToServiceBus");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("123", responseString);
        }

    }
}
