using Common.Filters;
using Common.Options;
using FirstService.Implementations;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [CustomRoute]
    public class HomeController : Controller
    {
        private readonly OAuthOptions _oauthOptions;

        public FirstServiceOptions _firstOptions { get; }

        public HomeController(IOptions<OAuthOptions> oauthOptions, IOptions<FirstServiceOptions> firstOptions)
        {
            _oauthOptions = oauthOptions.Value;
            _firstOptions = firstOptions.Value;
        }
        

        [HttpGet]
        [Route(nameof(Error))]
        public string Error(int val)
        {
            return "Error happened! number: " + val;
        }

        [HttpPost]
        [Route(nameof(PostDdata))]
        public string PostDdata(FirstModel model)
        {
            return model.Name;
        }

        //Authorization: Bearer token
        [HttpGet]
        [Route(nameof(Valid))]
        [Authorize]
        public string Valid()
        {
            var claims = User.Claims;
            return "the user is authenticated";
        }

        [HttpGet]
        [Route(nameof(GetToken))]
        public async Task<string> GetToken(CancellationToken cancellationToken)
        {
            DiscoveryClient disClient = new DiscoveryClient($"http://{_oauthOptions.host}/");
            disClient.Policy.RequireHttps = false;

            DiscoveryResponse disco = await disClient.GetAsync(cancellationToken);
            if (disco.IsError)
                throw new Exception("invalid endponit");

            TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "socialnetwork", "secret");

            TokenResponse tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("username", "password", "socialnetwork", cancellationToken);
            //TokenResponse tokenResponse = await tokenClient.RequestClientCredentialsAsync("socialnetwork");

            if (tokenResponse.IsError)
                throw new Exception("invalid token");


            HttpClient client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            HttpResponseMessage response = await client.GetAsync($"http://{_firstOptions.host}/api/home/valid", cancellationToken);
            string result = await response.Content.ReadAsStringAsync();

            return result + " & token: " + tokenResponse.AccessToken;
        }
    }
}