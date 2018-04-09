using Common.Implementations;
using ConsumerService.Business.Models;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class DataAddedConsumer : IConsumer<IPubSub>
    {
        public readonly IHttpService _httpService;

        public DataAddedConsumer(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            var user = _httpService.ToString();

            if (1 == 2) throw new Exception("Error1"); //goes to DataAddedFaultConsumer (if you have retry policy first restart happens!)

            await Task.Delay(5000);
        }
    }

    //when exeption raises
    public class DataAddedFaultConsumer : IConsumer<Fault<IPubSub>>
    {
        public async Task Consume(ConsumeContext<Fault<IPubSub>> context)
        {
            await Task.Delay(5000);
        }
    }
}
