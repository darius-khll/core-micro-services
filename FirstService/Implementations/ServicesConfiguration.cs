using Autofac;
using Common.Implementations;
using Common.Options;
using Common.Repositories;
using Common.Repositories.ServiceBus;
using ConsumerService.Business;
using ConsumerService.Business.Models;
using ConsumerService.Consumers;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Net.Http;

namespace FirstService.Implementations
{
    public static class ServicesConfiguration
    {
        public static void AutofacServices(this ContainerBuilder builder)
        {
            builder.RegisterType<CacheBusiness>().As<ICacheBusiness>().InstancePerLifetimeScope();
        }

        public static IBusControl ServiceBusRabbitmqConfiguration(this IServiceCollection services, IConfiguration Configuration)
        {
            string rabbitmqHost = Configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.host)}"];
            string user = Configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.user)}"];
            string password = Configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.password)}"];

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqHost}/"), h =>
                {
                    h.Username(user);
                    h.Password(password);
                });
            });

            return bus;
        }

        public static IServiceCollection ConfigureBus(this IServiceCollection services, IBusControl bus, IConfiguration Configuration)
        {
            services.AddSingleton<IPublishEndpoint>(bus);
            services.AddSingleton<ISendEndpointProvider>(bus);
            services.AddSingleton<IBus>(bus);

            services.AddScoped<IServiceBusRepository, ServiceBusRepository>();

            string rabbitmqHost = Configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.host)}"];
            var timeout = TimeSpan.FromSeconds(10);
            var serviceAddress = new Uri($"rabbitmq://{rabbitmqHost}/{nameof(SubmitOrderConsumer)}");
            services.AddScoped<IRequestClient<SubmitOrder, OrderAccepted>>(x =>
                new MessageRequestClient<SubmitOrder, OrderAccepted>(x.GetRequiredService<IBus>(), serviceAddress, timeout, timeout));

            bus.Start();

            return services;
        }

        public static IServiceCollection GeneralServices(this IServiceCollection services, IConfiguration Configuration)
        {
            string oauthHost = Configuration[$"{OAuthOptions.GetConfigName}:{nameof(OAuthOptions.host)}"];

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHttpService, HttpService>();

            services.AddScoped(typeof(IRedisCachingRepository<>), typeof(RedisCachingRepository<>)); //generic DI
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IFirstBusiness, FirstBusiness>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = $"http://{oauthHost}/";
                    options.ApiName = "socialnetwork";
                    options.RequireHttpsMetadata = false;
                });

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            return app;
        }
    }
}
