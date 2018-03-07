using Autofac;
using Common.Implementations;
using FirstService.Repository;
using FirstService.Repository.Implementations;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
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

        public static IServiceCollection ServiceBus(this IServiceCollection services)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://rabbitmq/"), h =>
                {
                    //h.Username("guest");
                    //h.Password("guest");
                });
            });

            services.AddSingleton<IPublishEndpoint>(bus);
            services.AddSingleton<ISendEndpointProvider>(bus);
            services.AddSingleton<IBus>(bus);

            var timeout = TimeSpan.FromSeconds(10);
            var serviceAddress = new Uri("rabbitmq://rabbitmq/order-service");

            services.AddScoped<IRequestClient<SubmitOrder, OrderAccepted>>(x =>
                new MessageRequestClient<SubmitOrder, OrderAccepted>(x.GetRequiredService<IBus>(), serviceAddress, timeout, timeout));

            bus.Start();

            return services;
        }

        public static IServiceCollection GeneralServices(this IServiceCollection services)
        {
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IFirstBusiness, FirstBusiness>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://oauthserver/";
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
