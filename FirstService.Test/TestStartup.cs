using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;

namespace FirstService.Test
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureRedis(IServiceCollection services)
        {
            services.AddScoped(provider => new RedisManagerPool("localhost:5050").GetClient());
        }
    }
}
