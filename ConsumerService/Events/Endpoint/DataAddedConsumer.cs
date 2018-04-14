using Common.Implementations;
using ConsumerService.Business.Models;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class DataAddedConsumer : IConsumer<IDataAdded>
    {
        public readonly IHttpService _httpService;

        public DataAddedConsumer(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task Consume(ConsumeContext<IDataAdded> context)
        {
            var user = _httpService.ToString();

            if (1 == 2) throw new Exception("Error1"); //goes to DataAddedFaultConsumer (if you have retry policy first restart happens!)

            await Task.Delay(1000);
        }
    }

    //when exeption raises
    public class DataAddedConsumerFault : IConsumer<Fault<IDataAdded>>
    {
        public async Task Consume(ConsumeContext<Fault<IDataAdded>> context)
        {
            await Task.Delay(1000);
        }
    }
}
