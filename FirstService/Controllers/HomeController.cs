﻿using Common.Implementations;
using FirstService.Implementations;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IHttpService _httpService;

        public HomeController(IHttpService httpService)
        {
            _httpService = httpService;
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
        [HttpGet]
        public async Task<string> GetToken()
        {
            DiscoveryClient disClient = new DiscoveryClient("http://oauthserver/");
            disClient.Policy.RequireHttps = false;

            DiscoveryResponse disco = await disClient.GetAsync();
            if (disco.IsError)
                throw new Exception("invalid endponit");

            TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "socialnetwork", "secret");

            TokenResponse tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("username", "password", "socialnetwork");
            //TokenResponse tokenResponse = await tokenClient.RequestClientCredentialsAsync("socialnetwork");

            if (tokenResponse.IsError)
                throw new Exception("invalid token");


            HttpClient client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            HttpResponseMessage response = await client.GetAsync("http://firstservice/home/valid");
            string result = await response.Content.ReadAsStringAsync();

            return result + " & token: " + tokenResponse.AccessToken;
        }
    }
}