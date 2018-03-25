using FirstService.Repository.Implementations;
using MassTransit;
using System.Threading.Tasks;

namespace ConsumerService
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            await Task.Delay(5000);
        }
    }
}
