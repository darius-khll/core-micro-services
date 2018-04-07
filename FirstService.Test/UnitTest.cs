using Common.Repositories;
using ConsumerService.Business;
using FirstService.Controllers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FirstService.Test
{
    public class UnitTest
    {

        public UnitTest()
        {
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(3, 5, 8)]
        [InlineData(-5, 0, -5)]
        [InlineData(-5, -2, -7)]
        public void SumMethodInFirstBusinessShouldWorks(int a, int b, int r)
        {
            IFirstBusiness firstBusiness = new FirstBusiness(null);
            int result = firstBusiness.Sum(a, b);
            Assert.Equal(r, result);
        }

        [Fact]
        public void ShuMethodShouldWorks()
        {
            IFirstBusiness firstBusiness = new FirstBusiness(null);
            int result = firstBusiness.Sum(1, 2);
            Assert.Equal(3, result);
        }

        [Theory]
        [InlineData("a", "b", "b")]
        [InlineData("c", "d", "d")]
        public async Task RedisShouldWorks(string user, string name, string res)
        {
            var mockDependency1 = new Mock<IRedisRepository>();
            mockDependency1.Setup(d => d.GetUser(user)).Returns(async () => await Task.Run(() => name));

            IFirstBusiness firstBusiness = new FirstBusiness(mockDependency1.Object);

            string result = await firstBusiness.UserHandlerAsync(user, name);

            Assert.Equal(res, result);
        }

        [Fact]
        public async Task CacheServiceShouldWorks()
        {
            var mockDependency1 = new Mock<ICacheBusiness>();
            string s = "done";
            mockDependency1.Setup((d) => d.RemoveCache()).Returns(async () => await Task.Run(() => s));

            var c = new CacheController(mockDependency1.Object);

            string result = await c.RemoveCache();

            Assert.Equal("done", result);
        }

    }
}
