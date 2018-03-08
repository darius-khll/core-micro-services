using Autofac;
using Autofac.Extensions.DependencyInjection;
using FirstService.Implementations;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace FirstService
{
    public class Startup
    {
        public IContainer ApplicationContainer { get; private set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var builder = new ContainerBuilder(); //Autofac

            services.Configure<RedisOptions>(Configuration.GetSection("redis"));

            ConfigureRedis(services);
            ConfigureDistributedCache(services);

            services.GeneralServices();
            builder.AutofacServices();

            services.AddMvc();
            services.AddSwaggerDocumentation();

            var bus = ConfigureRabbitmqHost(services);
            services.ConfigureBus(bus);

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);

        }

        public virtual IBusControl ConfigureRabbitmqHost(IServiceCollection services)
        {
            var bus = services.ServiceBus();
            return bus;
        }

        public virtual void ConfigureRedis(IServiceCollection services)
        {
            string redisHost = Configuration["redis:host"];
            services.AddScoped(provider => ConnectionMultiplexer.Connect(redisHost).GetDatabase());
        }

        public virtual void ConfigureDistributedCache(IServiceCollection services)
        {
            string redisHost = Configuration["redis:host"];
            string redisName = Configuration["redis:name"];

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisHost;
                options.InstanceName = redisName;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseSwaggerDocumentation();

            app.UseAuthentication();

            app.UseMvc();

            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}
