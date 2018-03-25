using FirstService.Repository.Implementations;
using MassTransit;
using System.Threading.Tasks;

namespace ConsumerService
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return context.RespondAsync<OrderAccepted>(new
            {
                context.Message.OrderId
            });
        }
    }
}
