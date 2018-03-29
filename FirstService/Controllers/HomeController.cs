﻿using Common.Implementations;
using FirstService.Implementations;
using FirstService.Repository.Implementations;
using IdentityModel.Client;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IHttpService _httpService;
        private readonly IRequestClient<SubmitOrder, OrderAccepted> _requestClient;
        private readonly IBus _bus;
        private IOptions<RabbitmqOptions> _rabbitmqOptions { get; }

        public HomeController(IHttpService httpService, IRequestClient<SubmitOrder, OrderAccepted> requestClient,
            IBus bus,  IOptions<RabbitmqOptions> rabbitmqOptions)
        {
            _httpService = httpService;
            _requestClient = requestClient;
            _bus = bus;
            _rabbitmqOptions = rabbitmqOptions;
        }

        [HttpGet]
        [Route(nameof(AddToServiceBus))]
        public async Task<string> AddToServiceBus(CancellationToken cancellationToken)
        {
            await _bus.Publish<IPubSub>(new PubSub { Message = "send message" });
            var endpoint = await _bus.GetSendEndpoint(new Uri($"rabbitmq://{_rabbitmqOptions.Value.host}/data-added"));
            await endpoint.Send<IPubSub>(new PubSub { Message = "data passed" });

            OrderAccepted result = await _requestClient.Request(new { OrderId = 123 }, cancellationToken);
            return result.OrderId;
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