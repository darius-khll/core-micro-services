using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Implementations;
using FirstService.Implementations;
using FirstService.Repository;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Net.Http;

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

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IFirstBusiness, FirstBusiness>();
            builder.RegisterType<CacheBusiness>().As<ICacheBusiness>().InstancePerLifetimeScope();
            //services.AddScoped<ICacheBusiness, CacheBusiness>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://oauthserver/";
                    options.ApiName = "socialnetwork";
                    options.RequireHttpsMetadata = false;
                });

            services.AddMvc();


            //builder.RegisterType<Repository>().As<IRepository>().InstancePerLifetimeScope();
            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);

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

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}
