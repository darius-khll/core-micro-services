using FirstService.Implementations;
using FirstService.Repository;
using FirstService.Repository.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Net.Http;

namespace FirstService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RedisOptions>(Configuration.GetSection("redis"));

            ConfigureRedis(services);
            ConfigureStagingServices(services);

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IFirstBusiness, FirstBusiness>();
            services.AddScoped<ICacheBusiness, CacheBusiness>();

            services.AddMvc();
        }

        public virtual void ConfigureRedis(IServiceCollection services)
        {
            string redisHost = Configuration["redis:host"];
            services.AddScoped(provider => ConnectionMultiplexer.Connect(redisHost).GetDatabase());
        }

        public virtual void ConfigureStagingServices(IServiceCollection services)
        {
            string redisHost = Configuration["redis:host"];
            string redisName = Configuration["redis:name"];

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisHost;
                options.InstanceName = redisName;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
