using ConsumerService.Business;
using ConsumerService.Business.Models;
using MassTransit;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public readonly IMongoBusiness _mongoBusiness;
        public readonly IPostgresBusiness _postgresBusiness;

        public PubSubConsumer(IMongoBusiness mongoBusiness, IPostgresBusiness postgresBusiness)
        {
            _mongoBusiness = mongoBusiness;
            _postgresBusiness = postgresBusiness;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            await _mongoBusiness.HandleLogic();
            await _postgresBusiness.HandleEfCoreLogic();
            //await _postgresBusiness.HandleDapperLogic();
        }
    }

    public class PubSubConsumerFault : IConsumer<Fault<IPubSub>>
    {
        public async Task Consume(ConsumeContext<Fault<IPubSub>> context)
        {
            await Task.Delay(1000);
        }
    }
}
