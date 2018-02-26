using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FirstService.Test
{
    public class TestStartup : Startup
    {
        public const string RediHost = "localhost:8184";

        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureRedis(IServiceCollection services)
        {
            services.AddScoped(provider => ConnectionMultiplexer.Connect(TestStartup.RediHost).GetDatabase());
        }

        public override void ConfigureDistributedCache(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = TestStartup.RediHost;
                options.InstanceName = "SampleInstance";
            });
        }
    }
}
