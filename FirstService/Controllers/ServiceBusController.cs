using Common.Filters;
using Common.Options;
using ConsumerService.Business.Implementations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [CustomRoute]
    public class ServiceBusController : Controller
    {
        private readonly IRequestClient<SubmitOrder, OrderAccepted> _requestClient;
        private readonly IBus _bus;
        private RabbitmqOptions _rabbitmqOptions { get; }

        public ServiceBusController(IRequestClient<SubmitOrder, OrderAccepted> requestClient,
            IBus bus, IOptions<RabbitmqOptions> rabbitmqOptions)
        {
            _requestClient = requestClient;
            _bus = bus;
            _rabbitmqOptions = rabbitmqOptions.Value;
        }
        /*
         *  AddToServiceBus => 
         *      1) publish
         *      2) using endpoint for specefic client
         *      3) request/response
         */
        [HttpGet]
        [Route(nameof(AddToServiceBus))]
        public async Task<string> AddToServiceBus(CancellationToken cancellationToken)
        {
            //1
            await _bus.Publish<IPubSub>(new PubSub { Message = "send message" });

            //2
            var endpoint = await _bus.GetSendEndpoint(new Uri($"rabbitmq://{_rabbitmqOptions.host}/data-added"));
            await endpoint.Send<IPubSub>(new PubSub { Message = "data passed" });

            //3
            OrderAccepted result = await _requestClient.Request(new { OrderId = 123 }, cancellationToken);
            return result.OrderId;
        }
    }
}
