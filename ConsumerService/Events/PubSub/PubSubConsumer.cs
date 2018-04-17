using ConsumerService.Business;
using ConsumerService.Business.Models;
using MassTransit;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public readonly IMongoBusiness _mongoBusiness;
        public readonly IDapperBusiness _dapperBusiness;

        public PubSubConsumer(IMongoBusiness mongoBusiness, IDapperBusiness dapperBusiness)
        {
            _mongoBusiness = mongoBusiness;
            _dapperBusiness = dapperBusiness;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            await _mongoBusiness.HandleLogic();
            //await _dapperBusiness.HandleLogic();
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
