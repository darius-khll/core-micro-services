using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace FirstService.Test
{
    public class TestStartup : Startup
    {
        public const string RediHost = "localhost:8184";
        public const string RabbitmqHost = "localhost:5672";

        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override IBusControl ConfigureRabbitmqHost(IServiceCollection services)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost:5672/"), h =>
                {
                    h.Username("user");
                    h.Password("password");
                });
            });

            return bus;
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
