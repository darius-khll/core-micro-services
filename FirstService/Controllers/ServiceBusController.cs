using Common.Filters;
using Common.Repositories.ServiceBus;
using ConsumerService.Business.Models;
using ConsumerService.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [CustomRoute]
    public class ServiceBusController : Controller
    {
        private readonly IRequestClient<SubmitOrder, OrderAccepted> _requestClient;
        public readonly IServiceBusRepository _busRepository;

        public ServiceBusController(IRequestClient<SubmitOrder, OrderAccepted> requestClient,
            IServiceBusRepository busRepository)
        {
            _requestClient = requestClient;
            _busRepository = busRepository;
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
            await _busRepository.Publish<IPubSub>(new PubSub { Message = "send message" });

            //2
            await _busRepository.SendToEndpoint<IDataAdded>(nameof(DataAddedConsumer), new DataAdded { Message = "data passed" });

            //3
            OrderAccepted result = await _requestClient.Request(new { OrderId = 123 }, cancellationToken);
            return result.OrderId;
        }
    }
}
