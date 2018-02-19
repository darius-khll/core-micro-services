using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FirstService.Test
{
    public class TestStartup : Startup
    {
        public static string _host = "localhost:5050";

        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureRedis(IServiceCollection services)
        {
            services.AddScoped(provider => ConnectionMultiplexer.Connect(TestStartup._host).GetDatabase());
        }

        public override void ConfigureDistributedCache(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = TestStartup._host;
                options.InstanceName = "SampleInstance";
            });
        }
    }
}
