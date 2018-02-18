using FirstService.Controllers;
using FirstService.Repository;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
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
            _client.Dispose();
            _server.Dispose();
        }

        [Fact]
        public async Task ErrorPageShouldReturnSomething()
        {
            var response = await _client.GetAsync("/home/error");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Error happened!", responseString);
        }

        [Fact]
        public async Task UserController_GetUser_ShouldReturnSomething()
        {
            var response = await _client.GetAsync("/user/getuser");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("bbb", responseString);
        }

        [Fact]
        public async Task UserController_Index_ShouldReturnSomething()
        {
            var response = await _client.GetAsync("/user/index");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("First Service requested: Second Service respond a user named: abc1 - bbb", responseString);
        }
        
    }
}
