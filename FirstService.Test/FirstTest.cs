using FirstService.Repository;
using Moq;
using System;
using System.Net.Http;
using Xunit;

namespace FirstService.Test
{
    public class FirstTest
    {

        public FirstTest()
        {
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(3, 5, 8)]
        [InlineData(-5, 0, -5)]
        [InlineData(-5, -2, -7)]
        public void SumMethodInFirstBusinessShouldWorks(int a, int b, int r)
        {
            IFirstBusiness firstBusiness = new FirstBusiness(null, null);
            int result = firstBusiness.Sum(a, b);
            Assert.Equal(r, result);
        }

        [Theory]
        [InlineData("a", "b", "b")]
        [InlineData("c", "d", "d")]
        public void RedisShouldWorks(string user, string name, string res)
        {
            var mockDependency1 = new Mock<IRedisRepository>();
            mockDependency1.Setup(d => d.GetUser(user)).Returns(name);

            IFirstBusiness firstBusiness = new FirstBusiness(null, mockDependency1.Object);

            string result = firstBusiness.UserTest(user, name);

            Assert.Equal(res, result);
        }
    }
}
