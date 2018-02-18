using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FirstService.Test
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureRedis(IServiceCollection services)
        {
            services.AddScoped(provider => ConnectionMultiplexer.Connect("localhost:5050").GetDatabase());
        }
    }
}
