using ConsumerService.Business;
using ConsumerService.Consumers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ConsumerService.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task SumMethodInFirstBusinessShouldWorks()
        {
            var mockDependency1 = new Mock<IMongoBusiness>();
            mockDependency1.Setup(d => d.HandleLogic()).Returns(async () => await Task.CompletedTask);

            var mockDependency2 = new Mock<IPostgresBusiness>();
            mockDependency2.Setup(d => d.HandleEfCoreLogic()).Returns(async () => await Task.CompletedTask);

            PubSubConsumer consumer = new PubSubConsumer(mockDependency1.Object, mockDependency2.Object);

            await consumer.Consume(null);

            Assert.True(true);
        }
    }
}
