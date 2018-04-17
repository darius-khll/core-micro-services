using ConsumerService.Business.Models;
using MassTransit;
using System.Threading.Tasks;
using Common.Implementations;

namespace ConsumerService.Consumers
{
    /// <summary>
    /// reuqest/response does not need to have fault implementation
    /// because they can handle in gateway
    /// SubmitOrder => passed from client
    /// OrderAccepted => responsed to client
    /// </summary>
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>, IRequestResponse<OrderAccepted>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await Task.Delay(1000);

            await context.RespondAsync<OrderAccepted>(new
            {
                context.Message.OrderId
            });
        }
    }
}
