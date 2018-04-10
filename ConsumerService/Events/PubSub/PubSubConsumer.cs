using ConsumerService.Business;
using ConsumerService.Business.Models;
using MassTransit;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public IMongoBusiness _mongoBusiness { get; set; }

        public PubSubConsumer(IMongoBusiness mongoBusiness)
        {
            _mongoBusiness = mongoBusiness;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            await _mongoBusiness.HandleLogic();
        }
    }
}
