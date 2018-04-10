using Common.Options;
using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Common.Repositories.ServiceBus
{
    public interface IServiceBusRepository
    {
        Task Publish<T>(T obj) where T : class;
        Task SendToEndpoint<T>(string endpoint, T obj) where T : class;
    }

    /*
     * just use for api gateway
     */
    public class ServiceBusRepository : IServiceBusRepository
    {
        private readonly IBus _bus;
        private RabbitmqOptions _rabbitmqOptions { get; }

        public ServiceBusRepository(IBus bus, IOptions<RabbitmqOptions> rabbitmqOptions)
        {
            _bus = bus;
            _rabbitmqOptions = rabbitmqOptions.Value;
        }

        //Publish<IPubSub>(new PubSub { Message = "send message" })
        public virtual async Task Publish<T>(T obj) where T : class
        {
            await _bus.Publish<T>(obj);
        }

        //SendToEndpoint<IPubSub>(new PubSub { Message = "data passed" });
        public virtual async Task SendToEndpoint<T>(string endpoint, T obj) where T : class
        {
            var end = await _bus.GetSendEndpoint(new Uri($"rabbitmq://{_rabbitmqOptions.host}/{endpoint}"));

            await end.Send<T>(obj);
        }

    }
}
