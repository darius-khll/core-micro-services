using Autofac;
using Common.Implementations;
using FirstService.Repository;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Net.Http;

namespace FirstService.Implementations
{
    public static class ServicesConfiguration
    {
        public static void AutofacServices(this ContainerBuilder builder)
        {
            builder.RegisterType<CacheBusiness>().As<ICacheBusiness>().InstancePerLifetimeScope();
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
